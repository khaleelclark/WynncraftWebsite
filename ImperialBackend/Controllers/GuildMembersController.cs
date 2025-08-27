using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/guildmembers")]
    public class GuildMembersController : ControllerBase
    {
        private readonly ImperialDbContext _context;
        public GuildMembersController(ImperialDbContext context) => _context = context;

        // GET: api/guildmembers/leaderboard?startDate=yyyy-MM-dd&endDate=yyyy-MM-dd
        [HttpGet("leaderboard")]
        public IActionResult GetLeaderboard([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var members = _context.GuildMembers
                .Include(m => m.PlayerHistoricalStats)
                .Include(m => m.Games)
                .ToList();

            var leaderboard = new List<object>();
            foreach (var m in members)
            {
                var statsInRange = m.PlayerHistoricalStats
                    .Where(s => s.SyncDate >= startDate && s.SyncDate <= endDate)
                    .OrderBy(s => s.SyncDate)
                    .ToList();

                int weekliesDiff = 0;
                int warsDiff = 0;
                int hoursDiff = 0;
                int raidCount = _context.RaidsCompleted
                    .Where(r => r.Uuid == m.Uuid && r.CompletedDate >= startDate && r.CompletedDate <= endDate)
                    .Count();

                if (statsInRange.Count >= 2)
                {
                    var first = statsInRange.First();
                    var last = statsInRange.Last();
                    weekliesDiff = last.WeekliesCompleted - first.WeekliesCompleted;
                    warsDiff = last.WarsCompleted - first.WarsCompleted;
                    hoursDiff = last.HoursPlayed - first.HoursPlayed;
                }
                // If 0 or 1 record, all stat diffs remain 0

                leaderboard.Add(new
                {
                    m.GuildMemberId,
                    m.MainUsername,
                    m.MinecraftUsername,
                    Uuid = m.Uuid,
                    WeekliesCompleted = weekliesDiff,
                    WarsCompleted = warsDiff,
                    HoursPlayed = hoursDiff,
                    RaidsCompleted = raidCount,
                    Games = m.Games.Select(g => g.Game?.GameName).Where(n => n != null).ToList()
                });
            }
            return Ok(leaderboard);
        }

        // GET: api/guildmembers
        [HttpGet]
        public IActionResult GetAll()
        {
            var members = _context.GuildMembers
                .Include(m => m.Games)
                .Include(m => m.Medals)
                .Include(m => m.Rank)
                .Select(m => new
                {
                    guild_member_id = m.GuildMemberId,
                    main_username = m.MainUsername,
                    discord_tag = m.DiscordTag,
                    minecraft_username = m.MinecraftUsername,
                    join_date = m.JoinDate,
                    uuid = m.Uuid,
                    wynncraft_rank = m.WynncraftRank,
                    hours_played = m.HoursPlayed,
                    wars_completed = m.WarsCompleted,
                    weeklies_completed = m.WeekliesCompleted,
                    last_synced = m.LastSynced,
                    rank_name = m.Rank != null ? m.Rank.RankName : null,
                    games = m.Games.Select(g => g.Game.GameName).ToList(),
                    medals = m.Medals.Select(md => md.Medal.MedalName).ToList(),
                })
                .ToList();
            return Ok(members);
        }

        // GET: api/guildmembers/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var member = _context.GuildMembers
                .Include(m => m.Games)
                    .ThenInclude(gmg => gmg.Game)
                .Include(m => m.Medals)
                    .ThenInclude(gmm => gmm.Medal)
                .Include(m => m.PlayerHistoricalStats)
                .Where(m => m.GuildMemberId == id)
                .FirstOrDefault();
            if (member == null) return NotFound();

            var oldestStat = member.PlayerHistoricalStats.OrderBy(s => s.SyncDate).FirstOrDefault();

            int warsDiff = 0;
            int weekliesDiff = 0;
            int hoursDiff = 0;
            if (oldestStat != null)
            {
                warsDiff = member.WarsCompleted - oldestStat.WarsCompleted;
                weekliesDiff = member.WeekliesCompleted - oldestStat.WeekliesCompleted;
                hoursDiff = member.HoursPlayed - oldestStat.HoursPlayed;
            }

            var response = new
            {
                member.GuildMemberId,
                member.MainUsername,
                member.DiscordTag,
                member.MinecraftUsername,
                member.JoinDate,
                member.Uuid,
                member.WynncraftRank,
                WarsCompleted = warsDiff,
                WeekliesCompleted = weekliesDiff,
                HoursPlayed = hoursDiff,
                member.LastSynced,
                Games = member.Games.Select(g => g.Game?.GameName).Where(n => n != null).ToList(),
                Medals = member.Medals.Select(md => md.Medal.MedalName).ToList()
            };
            return Ok(response);
        }
    }
}
