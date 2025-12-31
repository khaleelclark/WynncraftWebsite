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

            var existingMember = _context.GuildMembers.FirstOrDefault(m => m.Uuid == guildMember.Uuid);
            if (existingMember != null) return BadRequest("Guild member with this UUID already exists");

            // Validate RankId
            if (!_context.Ranks.Any(r => r.RankId == guildMember.Rank))
                return BadRequest("Invalid RankId.");

            GuildMember newMember = new GuildMember
            {
                MainUsername = guildMember.Name,
                DiscordTag = guildMember.DiscordTag,
                JoinDate = guildMember.JoinDate,
                Uuid = guildMember.Uuid,
                RankId = guildMember.Rank,
            };

            _context.GuildMembers.Add(newMember);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = newMember.GuildMemberId }, new
            {
                newMember.GuildMemberId,
                newMember.MainUsername,
                newMember.DiscordTag,
                newMember.JoinDate,
                newMember.Uuid,
                newMember.RankId
            });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GuildMemberPostDTO dto)
        {
            var existingMember = _context.GuildMembers
        .Include(m => m.Rank)
        .Include(m => m.Games)
            .ThenInclude(gm => gm.Game)
        .Include(m => m.Medals)
            .ThenInclude(mm => mm.Medal)
        .FirstOrDefault(m => m.GuildMemberId == id);

            if (existingMember == null) return NotFound();

            existingMember.DiscordTag = dto.DiscordTag;
            existingMember.MainUsername = dto.Name;
            existingMember.RankId = dto.Rank;
            existingMember.JoinDate = dto.JoinDate;

            _context.SaveChanges();
             _context.Entry(existingMember).Reference(m => m.Rank).Load();
            return Ok(new GuildMemberAdminGetDTO
            {
                DiscordTag = existingMember.DiscordTag,
                Id = existingMember.GuildMemberId,
                Name = existingMember.MainUsername,
                Uuid = existingMember.Uuid,
                Rank = new GenericGetDTO
                {
                    Id = existingMember.RankId,
                    Name = existingMember.Rank != null ? existingMember.Rank.RankName : "Error loading name"
                },
                JoinDate = existingMember.JoinDate,
                Games = existingMember.Games.Where(g => g.Game != null).Select(g => new GenericGetDTO
                    {
                        Id = g.Game.GameId,
                        Name = g.Game.GameName 
                    })
                    .ToList(),
                
                    Medals = existingMember.Medals.Where(m => m.Medal != null).Select(m => new GenericGetDTO
                    {
                        Id = m.Medal.MedalId,
                        Name = m.Medal.MedalName 
                    })
                    .ToList()
            });
        }

        
        // GET: api/guildmembers
        [HttpGet]
        public IActionResult GetAllPublic()
        {
            var members = _context.GuildMembers
                .Select(m => new GuildMemberPublicGetDTO
                {
                    Id = m.GuildMemberId,
                    DiscordTag = m.DiscordTag,
                    Name = m.MainUsername,
                    MinecraftUsername = m.MinecraftUsername,
                    RankName = m.Rank != null ? m.Rank.RankName: "Error loading name",
                    WynncraftRank = m.WynncraftRank,
                    Uuid = m.Uuid
                })
                .ToList();
            return Ok(members);
        }

        // GET: api/guildmembers/admin
        [HttpGet("admin")]
        public IActionResult GetAllAdmin()
        {
            var members = _context.GuildMembers
                .Include(m => m.Games)
                    .ThenInclude(gmg => gmg.Game)
                .Include(m => m.Medals)
                    .ThenInclude(gmm => gmm.Medal)
                .Select(m => new GuildMemberAdminGetDTO
                {
                    Id = m.GuildMemberId,
                    Name = m.MainUsername,
                    DiscordTag = m.DiscordTag,
                    Rank = new GenericGetDTO
                    {
                        Id = m.RankId,
                        Name = m.Rank != null ? m.Rank.RankName : "Error loading name"
                    },
                    Uuid = m.Uuid,
                    JoinDate = m.JoinDate,
                    Games = m.Games.Where(g => g.Game != null).Select(g => new GenericGetDTO
                    {
                        Id = g.Game.GameId,
                        Name = g.Game.GameName 
                    })
                    .ToList(),
                
                    Medals = m.Medals.Where(m => m.Medal != null).Select(m => new GenericGetDTO
                    {
                        Id = m.Medal.MedalId,
                        Name = m.Medal.MedalName 
                    })
                    .ToList()
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
                .Include(m => m.Rank)
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

            var response = new GuildMemberProfileGetDTO
            {
                Id = member.GuildMemberId,
                Name = member.MainUsername,
                DiscordTag = member.DiscordTag,
                MinecraftUsername = member.MinecraftUsername,
                JoinDate = member.JoinDate,
                Uuid = member.Uuid,
                WynncraftRank = member.WynncraftRank,
                RankName = member.Rank != null ? member.Rank.RankName: "Error loading name",
                WarsCompleted = warsDiff,
                HoursPlayed = hoursDiff,
                RaidsCompleted = raidsCompleted,
                LastSynced = member.LastSynced,
                Games = member.Games.Where(g => g.Game != null).Select(g => g.Game.GameName).ToList(),
                Medals = member.Medals.Where(m => m.Medal != null).Select(m => m.Medal.MedalName).ToList()
            };
            return Ok(response);
        }

        // GET: api/guildmembers/leaderboard?startDate=yyyy-MM-dd&endDate=yyyy-MM-dd
        [HttpGet("leaderboard")]
        public IActionResult GetLeaderboard([FromQuery] DateTimeOffset startDate, [FromQuery] DateTimeOffset endDate)
        {
            var members = _context.GuildMembers
                .Include(m => m.PlayerHistoricalStats)
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
                   SyncDate = DateTimeOffset.UtcNow // Use current time for ordering
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

                leaderboard.Add(new GuildMemberLeaderboardGetDTO
                {
                    Id = m.GuildMemberId,
                    Name = m.MainUsername,
                    MinecraftUsername = m.MinecraftUsername,
                    Uuid = m.Uuid,
                    WarsCompleted = warsDiff,
                    HoursPlayed = hoursDiff,
                    RaidsCompleted = raidCount,
                    LastSynced = m.LastSynced,
                });
            }
            return Ok(leaderboard);
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
