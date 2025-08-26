using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;



namespace ImperialBackend.Services
{
    public class GuildMemberSyncService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpClient _httpClient;

        public GuildMemberSyncService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _httpClient = new HttpClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateTime lastNightlySync = DateTime.MinValue;
            while (!stoppingToken.IsCancellationRequested)
            {
                await SyncGuildMembersFromApis();
                // Nightly sync at midnight
                if (DateTime.UtcNow.Hour == 0 && (DateTime.UtcNow - lastNightlySync).TotalHours > 23)
                {
                    await SyncNightlyStats();
                    lastNightlySync = DateTime.UtcNow;
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Sync every 5 minutes
            }
        }

        private async Task SyncGuildMembersFromApis()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ImperialDbContext>();
            var membersToSync = db.GuildMembers
                .Where(m => !string.IsNullOrEmpty(m.MinecraftUsername))
                .OrderBy(m => m.LastSynced ?? DateTime.MinValue)
                .Take(100)
                .ToList();
            foreach (var member in membersToSync)
            {
                // If UUID is set, try to sync username and skin from Mojang
                if (member.Uuid != Guid.Empty)
                {
                    // 1. Get UUID from Mojang API
                    try
                    {
                        var uuidResponse = await _httpClient.GetAsync($"https://api.minecraftservices.com/minecraft/profile/lookup/{member.Uuid}");
                        if (uuidResponse.IsSuccessStatusCode)
                        {
                            var uuidJson = await uuidResponse.Content.ReadAsStringAsync();
                            var uuidObj = System.Text.Json.JsonDocument.Parse(uuidJson).RootElement;
                            var uuidStr = uuidObj.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                            var mcName = uuidObj.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;
                            if (!string.IsNullOrEmpty(uuidStr))
                            {
                                member.Uuid = Guid.ParseExact(uuidStr, "N");
                            }
                            if (!string.IsNullOrEmpty(mcName) && member.MinecraftUsername != mcName)
                            {
                                member.MinecraftUsername = mcName;
                            }
                        }
                    }
                    catch { /* ignore errors, keep existing UUID */ }

                    // 2. Get skin from Mojang session server
                    if (member.Uuid != Guid.Empty)
                    {
                        try
                        {
                            var skinResponse = await _httpClient.GetAsync($"https://sessionserver.mojang.com/session/minecraft/profile/{member.Uuid.ToString("N")}");
                            if (skinResponse.IsSuccessStatusCode)
                            {
                                var skinJson = await skinResponse.Content.ReadAsStringAsync();
                                var skinObj = System.Text.Json.JsonDocument.Parse(skinJson).RootElement;
                                var properties = skinObj.TryGetProperty("properties", out var propArr) ? propArr.EnumerateArray() : default;
                                foreach (var prop in properties)
                                {
                                    if (prop.TryGetProperty("name", out var nameProp) && nameProp.GetString() == "textures")
                                    {
                                        var value = prop.TryGetProperty("value", out var valueProp) ? valueProp.GetString() : null;
                                        if (!string.IsNullOrEmpty(value))
                                        {
                                            // Decode base64 value
                                            var decoded = System.Text.Json.JsonDocument.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value))).RootElement;
                                            if (decoded.TryGetProperty("textures", out var texturesObj) && texturesObj.TryGetProperty("SKIN", out var skinObj2))
                                            {
                                                var skinUrl = skinObj2.TryGetProperty("url", out var urlProp) ? urlProp.GetString() : null;
                                                member.PlayerSkin = skinUrl;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch { /* ignore errors, keep existing skin */ }
                    }
                }
                // Wynncraft API sync (by UUID)
                try
                {
                    if (member.Uuid != Guid.Empty)
                    {
                        var wynnResponse = await _httpClient.GetAsync($"https://api.wynncraft.com/v3/player/{member.Uuid}");
                        if (wynnResponse.IsSuccessStatusCode)
                        {
                            var wynnJson = await wynnResponse.Content.ReadAsStringAsync();
                            var wynnObj = System.Text.Json.JsonDocument.Parse(wynnJson).RootElement;
                            // Get cumulative stats
                            member.HoursPlayed = wynnObj.TryGetProperty("playtime", out var playtimeProp) ? (int)playtimeProp.GetDouble() : 0;
                            if (wynnObj.TryGetProperty("globalData", out var globalData))
                            {
                                member.WarsCompleted = globalData.TryGetProperty("wars", out var warsProp) ? warsProp.GetInt32() : 0;
                            }
                            // Get player's rank in their current guild
                            if (wynnObj.TryGetProperty("guild", out var guildObj))
                            {
                                member.WynncraftRank = guildObj.TryGetProperty("rank", out var guildRankProp) ? guildRankProp.GetString() : null;
                            }
                        }
                    }
                }
                catch { /* ignore errors, keep existing Wynncraft stats */ }
                // Set LastSynced after all syncs
                member.LastSynced = DateTime.UtcNow;
            }
            db.SaveChanges();
        }

        public async Task RunNightlySyncManually()
        {
            await SyncNightlyStats();
        }

        private async Task SyncNightlyStats()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ImperialDbContext>();
            var members = db.GuildMembers.ToList();
            // Copy GuildMember data to PlayerHistoricalStats
            foreach (var member in members)
            {
                if (member.Uuid != Guid.Empty)
                {
                    db.PlayerHistoricalStats.Add(new PlayerHistoricalStat
                    {
                        WeekliesCompleted = member.WeekliesCompleted,
                        WarsCompleted = member.WarsCompleted,
                        HoursPlayed = member.HoursPlayed,
                        SyncDate = DateTime.UtcNow,
                        GuildMember = member
                    });
                }
            }
            await db.SaveChangesAsync();
        }
    }
}
