using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backend.Services
{
    public sealed class GuildMemberSyncService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GuildMemberSyncService> _logger;

        public GuildMemberSyncService(
            IServiceProvider serviceProvider,
            IHttpClientFactory httpClientFactory,
            ILogger<GuildMemberSyncService> logger
        )
        {
            _serviceProvider = serviceProvider;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateTimeOffset lastNightlySync = DateTimeOffset.MinValue;

            // Backoff when DB is down (prevents hammering)
            var backoff = TimeSpan.FromSeconds(5);
            var maxBackoff = TimeSpan.FromMinutes(2);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncGuildMembersFromApis(stoppingToken);

                    var now = DateTimeOffset.UtcNow;
                    // Run nightly stats once per UTC day, near midnight.
                    if (now.Hour == 0 && (now - lastNightlySync).TotalHours > 23)
                    {
                        await SyncNightlyStats(stoppingToken);
                        lastNightlySync = now;
                    }

                    // Success: reset backoff and sleep normal interval
                    backoff = TimeSpan.FromSeconds(5);
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (SqlException ex)
                {
                    _logger.LogWarning(
                        ex,
                        "DB unavailable; guild member sync backing off for {Delay}.",
                        backoff
                    );
                    await Task.Delay(backoff, stoppingToken);
                    backoff = TimeSpan.FromSeconds(
                        Math.Min(backoff.TotalSeconds * 2, maxBackoff.TotalSeconds)
                    );
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException)
                {
                    _logger.LogWarning(
                        ex,
                        "DB update failed (SQL down); backing off for {Delay}.",
                        backoff
                    );
                    await Task.Delay(backoff, stoppingToken);
                    backoff = TimeSpan.FromSeconds(
                        Math.Min(backoff.TotalSeconds * 2, maxBackoff.TotalSeconds)
                    );
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // graceful shutdown
                    return;
                }
                catch (Exception ex)
                {
                    // Don’t crash the host for unexpected errors; log and retry
                    _logger.LogError(
                        ex,
                        "Unexpected error in GuildMemberSyncService; continuing after short delay."
                    );
                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                }
            }
        }

        private async Task SyncGuildMembersFromApis(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<
                IDbContextFactory<WynncraftDbContext>
            >();
            await using var db = await dbFactory.CreateDbContextAsync(stoppingToken);

            // IMPORTANT: This query will throw SqlException when DB is down — now caught in ExecuteAsync
            // Batch the oldest synced members first to distribute API load fairly.
            var membersToSync = await db
                .GuildMembers.Where(m => m.Uuid != Guid.Empty)
                .OrderBy(m => m.LastSynced ?? DateTimeOffset.MinValue)
                .Take(100)
                .ToListAsync(stoppingToken);

            if (membersToSync.Count == 0)
                return;

            var http = _httpClientFactory.CreateClient();

            foreach (var member in membersToSync)
            {
                stoppingToken.ThrowIfCancellationRequested();

                // ---- External APIs should not crash the worker ----
                // Mojang profile lookup
                try
                {
                    // Mojang returns canonical UUID and current username.
                    var uuidResponse = await http.GetAsync(
                        $"https://api.minecraftservices.com/minecraft/profile/lookup/{member.Uuid}",
                        stoppingToken
                    );

                    if (uuidResponse.IsSuccessStatusCode)
                    {
                        var uuidJson = await uuidResponse.Content.ReadAsStringAsync(stoppingToken);
                        var root = System.Text.Json.JsonDocument.Parse(uuidJson).RootElement;

                        var uuidStr = root.TryGetProperty("id", out var idProp)
                            ? idProp.GetString()
                            : null;
                        var mcName = root.TryGetProperty("name", out var nameProp)
                            ? nameProp.GetString()
                            : null;

                        if (!string.IsNullOrEmpty(uuidStr))
                            member.Uuid = Guid.ParseExact(uuidStr, "N");

                        if (!string.IsNullOrEmpty(mcName) && member.MinecraftUsername != mcName)
                            member.MinecraftUsername = mcName;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(
                        ex,
                        "Mojang profile lookup failed for member {Id}.",
                        member.GuildMemberId
                    );
                }

                // Wynncraft API
                try
                {
                    if (member.Uuid != Guid.Empty)
                    {
                        // Wynncraft supplies playtime, wars, and guild rank.
                        var wynnResponse = await http.GetAsync(
                            $"https://api.wynncraft.com/v3/player/{member.Uuid}",
                            stoppingToken
                        );

                        if (wynnResponse.IsSuccessStatusCode)
                        {
                            var wynnJson = await wynnResponse.Content.ReadAsStringAsync(
                                stoppingToken
                            );
                            var wynnObj = System.Text.Json.JsonDocument.Parse(wynnJson).RootElement;

                            member.HoursPlayed = wynnObj.TryGetProperty(
                                "playtime",
                                out var playtimeProp
                            )
                                ? (int)playtimeProp.GetDouble()
                                : 0;

                            if (wynnObj.TryGetProperty("globalData", out var globalData))
                            {
                                member.WarsCompleted = globalData.TryGetProperty(
                                    "wars",
                                    out var warsProp
                                )
                                    ? warsProp.GetInt32()
                                    : 0;
                            }

                            if (wynnObj.TryGetProperty("guild", out var guildObj))
                            {
                                member.WynncraftRank = guildObj.TryGetProperty(
                                    "rank",
                                    out var guildRankProp
                                )
                                    ? guildRankProp.GetString()
                                    : null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(
                        ex,
                        "Wynncraft sync failed for member {Id}.",
                        member.GuildMemberId
                    );
                }

                // Mark as synced even if one of the APIs failed.
                member.LastSynced = DateTimeOffset.UtcNow;
            }

            try
            {
                // Persist all member updates in one transaction.
                await db.SaveChangesAsync(stoppingToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                // ignore this cycle
            }
        }

        private async Task SyncNightlyStats(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<
                IDbContextFactory<WynncraftDbContext>
            >();
            await using var db = await dbFactory.CreateDbContextAsync(stoppingToken);

            var members = await db.GuildMembers.ToListAsync(stoppingToken);

            foreach (var member in members)
            {
                if (member.Uuid != Guid.Empty)
                {
                    db.PlayerHistoricalStats.Add(
                        new PlayerHistoricalStat
                        {
                            WeekliesCompleted = member.WeekliesCompleted,
                            WarsCompleted = member.WarsCompleted,
                            HoursPlayed = member.HoursPlayed,
                            SyncDate = DateTimeOffset.UtcNow,
                            GuildMemberId = member.GuildMemberId,
                        }
                    );
                }
            }

            try
            {
                await db.SaveChangesAsync(stoppingToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                // ignore
            }
        }
    }
}
