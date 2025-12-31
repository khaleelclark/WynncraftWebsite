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

            // (Optional) validate medal IDs
            if (guildMember.Medals != null && guildMember.Medals.Any())
            {
                var invalidMedalIds = guildMember.Medals
                    .Except(_context.Medals.Select(m => m.MedalId))
                    .ToList();

                if (invalidMedalIds.Any())
                    return BadRequest($"Invalid MedalIds: {string.Join(", ", invalidMedalIds)}");
            }

            // (Optional) validate game IDs
            if (guildMember.Games != null && guildMember.Games.Any())
            {
                var invalidGameIds = guildMember.Games
                    .Except(_context.Games.Select(g => g.GameId))
                    .ToList();

                if (invalidGameIds.Any())
                    return BadRequest($"Invalid GameIds: {string.Join(", ", invalidGameIds)}");
            }

            GuildMember newMember = new GuildMember
            {
                MainUsername = guildMember.Name,
                DiscordTag = guildMember.DiscordTag,
                JoinDate = guildMember.JoinDate,
                Uuid = guildMember.Uuid,
                RankId = guildMember.Rank,
                Medals = new List<GuildMemberMedal>(),
                Games = new List<GuildMemberGame>()
            };

                    // Add medals from DTO
            if (guildMember.Medals != null)
            {
                foreach (var medalId in guildMember.Medals.Distinct())
                {
                    newMember.Medals.Add(new GuildMemberMedal
                    {
                        MedalId = medalId
                    });
                }
            }

            // Add games from DTO
            if (guildMember.Games != null)
            {
                foreach (var gameId in guildMember.Games.Distinct())
                {
                    newMember.Games.Add(new GuildMemberGame
                    {
                        GameId = gameId
                    });
                }
            }

            _context.GuildMembers.Add(newMember);
            _context.SaveChanges();

             var createdMember = _context.GuildMembers
            .Include(m => m.Rank)
            .Include(m => m.Games).ThenInclude(gm => gm.Game)
            .Include(m => m.Medals).ThenInclude(mm => mm.Medal)
            .First(m => m.GuildMemberId == newMember.GuildMemberId);

            return Ok(new GuildMemberAdminGetDTO
            {
                DiscordTag = createdMember.DiscordTag,
                Id = createdMember.GuildMemberId,
                Name = createdMember.MainUsername,
                Uuid = createdMember.Uuid,
                Rank = new GenericGetDTO
                {
                    Id = createdMember.RankId,
                    Name = createdMember.Rank != null ? createdMember.Rank.RankName : "Error loading name"
                },
                JoinDate = createdMember.JoinDate,
                Games = createdMember.Games
                .Where(g => g.Game != null)
                .Select(g => new GenericGetDTO
                {
                    Id = g.Game.GameId,
                    Name = g.Game.GameName
                })
                .ToList(),
                Medals = createdMember.Medals
                .Where(m => m.Medal != null)
                .Select(m => new GenericGetDTO
                {
                    Id = m.Medal.MedalId,
                    Name = m.Medal.MedalName
                })
                .ToList()
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
            var existingMedalIds = existingMember.Medals
            .Select(mm => mm.MedalId)
            .ToList();
            var existingGameIds = existingMember.Games
            .Select(gg => gg.GameId)
            .ToList();


    // target medal IDs from the DTO (null-safe)
    var targetMedalIds = dto.Medals ?? new List<int>();
    var targetGameIds = dto.Games ?? new List<int>();

    // medals to remove
    var medalsToRemove = existingMember.Medals
        .Where(mm => !targetMedalIds.Contains(mm.MedalId))
        .ToList();
    var gamesToRemove = existingMember.Games
        .Where(gg => !targetGameIds.Contains(gg.GameId))
        .ToList();

    _context.GuildMemberMedals.RemoveRange(medalsToRemove);
    _context.GuildMemberGames.RemoveRange(gamesToRemove);

    // medals to add (IDs in DTO that member doesn't currently have)
    var medalIdsToAdd = targetMedalIds
        .Except(existingMedalIds)
        .ToList();

        var gameIdsToAdd = targetGameIds
        .Except(existingGameIds)
        .ToList();

    foreach (var medalId in medalIdsToAdd)
    {
        existingMember.Medals.Add(new GuildMemberMedal
        {
            GuildMemberId = existingMember.GuildMemberId,
            MedalId = medalId
        });
    }

    foreach (var gameId in gameIdsToAdd)
    {
        existingMember.Games.Add(new GuildMemberGame
        {
            GuildMemberId = existingMember.GuildMemberId,
            GameId = gameId
        });
    }

            _context.SaveChanges();
             _context.Entry(existingMember).Reference(m => m.Rank).Load();

             _context.Entry(existingMember)
            .Collection(m => m.Medals)
            .Query()
            .Include(mm => mm.Medal)
            .Load();

            _context.Entry(existingMember)
            .Collection(m => m.Games)
            .Query()
            .Include(mm => mm.Game)
            .Load();


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

                Games = existingMember.Games
                .Select(g => new GenericGetDTO
                    {
                        Id = g.GameId,
                        Name = g.Game!.GameName 
                    })
                    .ToList(),
                
                Medals = existingMember.Medals
                .Select(m => new GenericGetDTO
                {
                    Id = m.MedalId,
                    Name = m.Medal!.MedalName
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
