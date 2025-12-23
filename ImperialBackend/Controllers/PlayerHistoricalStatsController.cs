using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/playerhistoricalstats")]
    public class PlayerHistoricalStatsController : ControllerBase
    {
        private readonly ImperialDbContext _context;
        public PlayerHistoricalStatsController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
{
    var stats = _context.PlayerHistoricalStats
        .Include(s => s.GuildMember)
        .OrderByDescending(s => s.SyncDate)
        .Select(s => new PlayerHistoricalStatReadDTO
        {
            StatHistoryId = s.StatHistoryId,
            WeekliesCompleted = s.WeekliesCompleted,
            WarsCompleted = s.WarsCompleted,
            HoursPlayed = s.HoursPlayed,
            SyncDate = s.SyncDate,
            GuildMemberId = s.GuildMemberId,
            MinecraftUsername = s.GuildMember.MinecraftUsername,
            MainUsername = s.GuildMember.MainUsername,
            DiscordTag = s.GuildMember.DiscordTag
        })
        .ToList();

    return Ok(stats);
}

        [HttpGet("by-member/{guildMemberId}")]
        public IActionResult GetStatByGuildMemberId(int guildMemberId)
        {
            var stat = _context.PlayerHistoricalStats
                .Include(s => s.GuildMember)
                //get by GuildMemberId, NOT stat id
                .Where(s => s.GuildMemberId == guildMemberId)
                .OrderByDescending(s => s.SyncDate)
                .Select(s => new PlayerHistoricalStatReadDTO
                {
                    StatHistoryId = s.StatHistoryId,
                    WeekliesCompleted = s.WeekliesCompleted,
                    WarsCompleted = s.WarsCompleted,
                    HoursPlayed = s.HoursPlayed,
                    SyncDate = s.SyncDate,
                    GuildMemberId = s.GuildMemberId,
                    MinecraftUsername = s.GuildMember.MinecraftUsername,
                    MainUsername = s.GuildMember.MainUsername,
                    DiscordTag = s.GuildMember.DiscordTag
                })
                .FirstOrDefault();

            if (stat == null) return NotFound();
            return Ok(stat);
        }

        [HttpPost]
        public IActionResult Post([FromBody] PlayerHistoricalStatWriteDTO dto)
        {

            if (dto.GuildMemberId <= 0) return BadRequest("GuildMemberId is required.");

            var stat = new PlayerHistoricalStat
            {
                WeekliesCompleted = dto.WeekliesCompleted,
                WarsCompleted = dto.WarsCompleted,
                HoursPlayed = dto.HoursPlayed,
                SyncDate = dto.SyncDate,
                GuildMemberId = dto.GuildMemberId
            };

            _context.PlayerHistoricalStats.Add(stat);
            _context.SaveChanges();

            var read = _context.PlayerHistoricalStats
                .Include(s => s.GuildMember)
                .Where(s => s.StatHistoryId == stat.StatHistoryId)
                .Select(s => new PlayerHistoricalStatReadDTO
                {
                    StatHistoryId = s.StatHistoryId,
                    WeekliesCompleted = s.WeekliesCompleted,
                    WarsCompleted = s.WarsCompleted,
                    HoursPlayed = s.HoursPlayed,
                    SyncDate = s.SyncDate,
                    GuildMemberId = s.GuildMemberId,
                    MinecraftUsername = s.GuildMember.MinecraftUsername,
                    MainUsername = s.GuildMember.MainUsername,
                    DiscordTag = s.GuildMember.DiscordTag
                })
                .First();

            return CreatedAtAction(
            nameof(GetStatByGuildMemberId),
            new { guildMemberId = stat.GuildMemberId },
            read 
            );
        }
    }
}
