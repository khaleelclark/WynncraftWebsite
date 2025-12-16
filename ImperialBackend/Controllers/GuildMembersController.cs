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

        [HttpPost]
        public IActionResult Post([FromBody] GuildMemberPostDTO guildMember)
        {
            GuildMember newMember = new GuildMember
            {
                MainUsername = guildMember.MainUsername,
                DiscordTag = guildMember.DiscordTag,
                JoinDate = guildMember.JoinDate,
                Uuid = guildMember.Uuid,
                RankId = guildMember.RankId,
            };

            _context.GuildMembers.Add(newMember);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = newMember.GuildMemberId }, guildMember);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GuildMemberPutDTO dto)
        {
            var existingMember = _context.GuildMembers.Find(id);
            if (existingMember == null) return NotFound();

            existingMember.DiscordTag = dto.DiscordTag;
            existingMember.MainUsername = dto.MainUsername;
            existingMember.RankId = dto.RankId;

            _context.SaveChanges();
            return NoContent();
        }

        // GET: api/guildmembers/leaderboard?startDate=yyyy-MM-dd&endDate=yyyy-MM-dd
        [HttpGet("leaderboard")]
        public IActionResult GetLeaderboard([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var members = _context.GuildMembers
                .Include(m => m.PlayerHistoricalStats)
                .Include(m => m.Games)
                .ToList();

            var raidCounts = _context.RaidsCompleted
                .Where(r => r.CompletedDate >= startDate && r.CompletedDate <= endDate)
                .GroupBy(r => r.Uuid)
                .Select(g => new
                {
                    Uuid = g.Key,
                    Count = g.Count()
                })
                .ToDictionary(x => x.Uuid, x => x.Count);

            var leaderboard = new List<object>();
            foreach (var m in members)
            {
                // Build statsInRange with current stats appended as the latest entry
                var statsInRange = m.PlayerHistoricalStats
                    .Where(s => s.SyncDate >= startDate && s.SyncDate <= endDate)
                    .OrderBy(s => s.SyncDate)
                    .ToList();

                // Always include current stats as the latest entry
                statsInRange.Add(new Models.PlayerHistoricalStat
                {
                    WeekliesCompleted = m.WeekliesCompleted,
                    WarsCompleted = m.WarsCompleted,
                    HoursPlayed = m.HoursPlayed,
                    SyncDate = DateTime.Now // Use current time for ordering
                });

                int weekliesDiff = 0;
                int warsDiff = 0;
                int hoursDiff = 0;
                int raidCount = raidCounts.TryGetValue(m.Uuid, out var count)
                ? count
                : 0;


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
                    m.Uuid,
                    WeekliesCompleted = weekliesDiff,
                    WarsCompleted = warsDiff,
                    HoursPlayed = hoursDiff,
                    RaidsCompleted = raidCount,
                    m.LastSynced, 
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

            int raidsCompleted = _context.RaidsCompleted.Count(r => r.Uuid == member.Uuid);

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
                RaidsCompleted = raidsCompleted,
                member.LastSynced,
                Games = member.Games.Select(g => g.Game?.GameName).Where(n => n != null).ToList(),
                Medals = member.Medals.Select(md => md.Medal.MedalName).ToList()
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var member = _context.GuildMembers.Find(id);
            if (member == null) return NotFound();
            _context.GuildMembers.Remove(member);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
