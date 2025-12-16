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
        
        // [HttpGet("{id}")]
        // public IActionResult GetById(int id)
        // {
        //     var stat = _context.PlayerHistoricalStats.FirstOrDefault(s => s.StatHistoryId == id);
        //     if (stat == null) return NotFound();
        //     return Ok(stat);
        // }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var stat = _context.PlayerHistoricalStats
                .Include(s => s.GuildMember)
                .Where(s => s.StatHistoryId == id)
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


        //[HttpPost]
        // public IActionResult Post([FromBody] PlayerHistoricalStat stat)
        // {
        //     _context.PlayerHistoricalStats.Add(stat);
        //     _context.SaveChanges();
        //     return CreatedAtAction(nameof(GetById), new { id = stat.StatHistoryId }, stat);
        // }
        [HttpPost]
        public IActionResult Post([FromBody] PlayerHistoricalStatWriteDTO dto)
        {
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

            return CreatedAtAction(nameof(GetById), new { id = stat.StatHistoryId }, read);
        }


        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] PlayerHistoricalStatWriteDTO dto)
        {
            var stat = _context.PlayerHistoricalStats.Find(id);
            if (stat == null) return NotFound();

            stat.WeekliesCompleted = dto.WeekliesCompleted;
            stat.WarsCompleted = dto.WarsCompleted;
            stat.HoursPlayed = dto.HoursPlayed;
            stat.SyncDate = dto.SyncDate;
            stat.GuildMemberId = dto.GuildMemberId;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var stat = _context.PlayerHistoricalStats.Find(id);
            if (stat == null) return NotFound();
            _context.PlayerHistoricalStats.Remove(stat);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
