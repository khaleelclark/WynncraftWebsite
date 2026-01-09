using System.Text;
using System.Text.Json;
using ImperialBackend.Contracts;
using ImperialBackend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RMQChannel = RabbitMQ.Client.IChannel;
using RMQConnection = RabbitMQ.Client.IConnection;
using RMQConnectionFactory = RabbitMQ.Client.ConnectionFactory;

namespace ImperialBackend.Messaging;

public sealed class RaidCompletedConsumer : BackgroundService
{
    private readonly ILogger<RaidCompletedConsumer> _logger;
    private readonly IDbContextFactory<ImperialDbContext> _dbFactory;

    private RMQConnection? _connection;
    private RMQChannel? _channel;

    // Secrets (move to env vars later)
    private const string HostName = "raid-rabbit";
    private const string UserName = "imperial-website";
    private const string Password = "WebsiteStrongPassword";
    private const string VirtualHost = "imperial";

    private const string QueueName = "raids.completed.q";

    public RaidCompletedConsumer(
        IDbContextFactory<ImperialDbContext> dbFactory,
        ILogger<RaidCompletedConsumer> logger
    )
    {
        _dbFactory = dbFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = TimeSpan.FromSeconds(2);
        var maxDelay = TimeSpan.FromSeconds(30);

        // Throttle noisy outage logs:
        // - Warn on attempt 1,6,11,... (every 5th attempt)
        // - Debug on all other attempts
        var connectAttempts = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await EnsureRabbitAsync(stoppingToken);

                if (_channel is null)
                    throw new InvalidOperationException("RabbitMQ channel not initialized.");

                await _channel.BasicQosAsync(0, 1, false);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += (_, ea) => OnMessageAsync(ea, stoppingToken);

                await _channel.BasicConsumeAsync(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer
                );

                _logger.LogInformation("Consuming RabbitMQ queue: {Queue}", QueueName);

                // Success: reset backoff + attempt counter
                delay = TimeSpan.FromSeconds(2);
                connectAttempts = 0;

                // Keep alive until disconnect or shutdown
                await WaitForDisconnectAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                connectAttempts++;

                // ✅ Reduced log spam: no stack trace in warning, debug for most retries
                if (connectAttempts % 5 == 1)
                {
                    _logger.LogWarning(
                        "RabbitMQ unreachable (attempt {Attempt}); retrying in {Delay}. Message: {Message}",
                        connectAttempts,
                        delay,
                        ex.Message
                    );
                }
                else
                {
                    _logger.LogDebug(
                        "RabbitMQ unreachable (attempt {Attempt}); retrying in {Delay}. Message: {Message}",
                        connectAttempts,
                        delay,
                        ex.Message
                    );
                }

                await SafeCloseRabbitAsync();
                await Task.Delay(delay, stoppingToken);

                delay = TimeSpan.FromSeconds(
                    Math.Min(delay.TotalSeconds * 2, maxDelay.TotalSeconds)
                );
            }
        }

        await SafeCloseRabbitAsync();
    }

    private async Task OnMessageAsync(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
    {
        if (_channel is null)
            return;

        string bodyText = "";

        try
        {
            bodyText = Encoding.UTF8.GetString(ea.Body.ToArray());

            var msg =
                JsonSerializer.Deserialize<RaidCompletedMessage>(
                    bodyText,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? throw new InvalidOperationException("Message deserialized to null.");

            if (msg.MinecraftUsernames is null || msg.MinecraftUsernames.Count == 0)
                throw new InvalidOperationException("minecraftUsernames cannot be empty.");

            await using var db = await _dbFactory.CreateDbContextAsync(stoppingToken);
            await using var tx = await db.Database.BeginTransactionAsync(stoppingToken);

            var raid =
                await db.Raids.FindAsync(new object[] { msg.RaidId }, stoppingToken)
                ?? throw new InvalidOperationException($"Unknown RaidId {msg.RaidId}");

            var raidCompleted = new RaidCompleted
            {
                RaidId = msg.RaidId,
                CompletedDate = msg.CompletedDate.UtcDateTime,
                RaidInstances = new List<RaidInstance>(),
            };

            var distinctNames = msg
                .MinecraftUsernames.Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var members = await db
                .GuildMembers.Where(m => m.MinecraftUsername != null)
                .Select(m => new { m.GuildMemberId, m.MinecraftUsername })
                .ToListAsync(stoppingToken);

            var notFound = new List<string>();

            foreach (var username in distinctNames)
            {
                var member = members.FirstOrDefault(m =>
                    string.Equals(m.MinecraftUsername, username, StringComparison.OrdinalIgnoreCase)
                );

                if (member == null)
                {
                    notFound.Add(username);
                    continue;
                }

                raidCompleted.RaidInstances.Add(
                    new RaidInstance { GuildMemberId = member.GuildMemberId }
                );
            }

            db.RaidsCompleted.Add(raidCompleted);
            await db.SaveChangesAsync(stoppingToken);
            await tx.CommitAsync(stoppingToken);

            _logger.LogInformation(
                "RaidCompleted processed: RaidId={RaidId} Instances={Count} Missing=[{Missing}]",
                raid.RaidId,
                raidCompleted.RaidInstances.Count,
                string.Join(", ", notFound)
            );

            // RabbitMQ calls here do NOT take CancellationToken in many client versions
            await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        }
        catch (SqlException ex)
        {
            _logger.LogWarning(ex, "DB unavailable; requeueing raid.completed message.");

            await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException)
        {
            _logger.LogWarning(ex, "DB update failed; requeueing raid.completed message.");

            await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process raid.completed. Body: {Body}", bodyText);

            // Poison message → do NOT requeue
            await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
        }
    }

    private async Task EnsureRabbitAsync(CancellationToken ct)
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
            return;

        await SafeCloseRabbitAsync();

        var factory = new RMQConnectionFactory
        {
            HostName = HostName,
            UserName = UserName,
            Password = Password,
            VirtualHost = VirtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
        };

        // Some RabbitMQ.Client versions accept CancellationToken; some don't.
        // If you get a compile error here, change to: _connection = await factory.CreateConnectionAsync();
        _connection = await factory.CreateConnectionAsync(ct);

        _channel = await _connection.CreateChannelAsync();

        _logger.LogInformation("RabbitMQ connected for queue {Queue}", QueueName);
    }

    private async Task WaitForDisconnectAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (_connection is null || !_connection.IsOpen || _channel is null || !_channel.IsOpen)
                return;

            await Task.Delay(TimeSpan.FromSeconds(2), ct);
        }
    }

    private async Task SafeCloseRabbitAsync()
    {
        try
        {
            if (_channel is not null)
                await _channel.CloseAsync();
        }
        catch { }

        try
        {
            if (_connection is not null)
                await _connection.CloseAsync();
        }
        catch { }

        _channel = null;
        _connection = null;
    }
}
