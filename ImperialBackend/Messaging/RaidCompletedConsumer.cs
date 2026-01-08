using System.Text;
using System.Text.Json;
using ImperialBackend.Contracts;
using ImperialBackend.Models;
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

    // Needs Secret
    private const string HostName = "raid-rabbit";
    private const string UserName = "imperial-website";
    private const string Password = "WebsiteStrongPassword";
    private const string VirtualHost = "imperial";

    private const string ExchangeName = "raids.exchange";
    private const string QueueName = "raids.completed.q";
    private const string RoutingKey = "raid.completed";

    public RaidCompletedConsumer(
        IDbContextFactory<ImperialDbContext> dbFactory,
        ILogger<RaidCompletedConsumer> logger
    )
    {
        _dbFactory = dbFactory;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new RMQConnectionFactory
        {
            HostName = HostName,
            UserName = UserName,
            Password = Password,
            VirtualHost = VirtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync();

        await _channel.BasicQosAsync(0, 1, false);

        _logger.LogInformation("RabbitMQ consumer connected: {Queue}", QueueName);

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null)
            throw new InvalidOperationException("RabbitMQ channel not initialized.");

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
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

                var completedDateUtc = msg.CompletedDate.UtcDateTime;

                var raidCompleted = new RaidCompleted
                {
                    RaidId = msg.RaidId,
                    CompletedDate = completedDateUtc,
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
                        string.Equals(
                            m.MinecraftUsername,
                            username,
                            StringComparison.OrdinalIgnoreCase
                        )
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
                    "RaidCompleted processed: RaidId={RaidId} Instances={Count} NotFound=[{Missing}]",
                    raid.RaidId,
                    raidCompleted.RaidInstances.Count,
                    string.Join(", ", notFound)
                );

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process raid.completed. Body: {Body}", bodyText);

                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        return _channel.BasicConsumeAsync(QueueName, autoAck: false, consumer, stoppingToken);
    }
}
