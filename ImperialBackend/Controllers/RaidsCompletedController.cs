using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;
using ImperialBackend.DTOs;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/raidscompleted")]
    public class RaidsCompletedController : ControllerBase
    {
        private readonly ImperialDbContext _context;
        public RaidsCompletedController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            var raids = _context.RaidsCompleted
                .Select(r => new
                {
                    r.RaidCompletedId,
                    r.RaidId,
                    r.RaidInstanceId,
                    r.Uuid,
                    r.CompletedDate
                })
                .ToList();
            return Ok(raids);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var raidCompleted = _context.RaidsCompleted
                .AsNoTracking()
                .Where(r => r.RaidCompletedId == id)
                .Select(r => new
                {
                    r.RaidCompletedId,
                    r.RaidId,
                    r.RaidInstanceId,
                    r.Uuid,
                    r.CompletedDate
                })
                .FirstOrDefault();

            if (raidCompleted == null) return NotFound();
            return Ok(raidCompleted);
        }
       
[HttpPost("raid-bot-report")]
public IActionResult SyncFromBot([FromBody] RaidBotReportDTO dto)
{
    if (dto.MinecraftUsernames == null || dto.MinecraftUsernames.Count == 0)
        return BadRequest("minecraftUsernames must contain at least one player.");

    var raid = _context.Raids.Find(dto.RaidId);
    if (raid == null)
        return BadRequest($"Unknown RaidId {dto.RaidId}");

    var completedDateUtc = (dto.CompletedDate ?? DateTimeOffset.UtcNow).UtcDateTime;

    int newRaidInstanceId =
        (_context.RaidsCompleted.Any()
            ? _context.RaidsCompleted.Max(r => r.RaidInstanceId)
            : 0) + 1;

    var created = new List<object>();
    var notFoundUsers = new List<string>();

    foreach (var username in dto.MinecraftUsernames.Distinct(StringComparer.OrdinalIgnoreCase))
    {
        var member = _context.GuildMembers
            .FirstOrDefault(m => m.MinecraftUsername == username);

        if (member == null)
        {
            notFoundUsers.Add(username);
            continue;
        }

        var rc = new RaidCompleted
        {
            RaidId = dto.RaidId,
            RaidInstanceId = newRaidInstanceId,
            Uuid = member.Uuid,
            CompletedDate = completedDateUtc,
            GuildMember = member
        };

        _context.RaidsCompleted.Add(rc);

        created.Add(new
        {
            username,
            member.Uuid,
            raid.RaidName,
            newRaidInstanceId,
            completedDateUtc
        });
    }

    _context.SaveChanges();

    return Ok(new
    {
        raidId = raid.RaidId,
        raidName = raid.RaidName,
        raidInstanceId = newRaidInstanceId,
        completedDateUtc,
        createdCount = created.Count,
        notFoundUsers
    });
}

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var raidCompleted = _context.RaidsCompleted.Find(id);
            if (raidCompleted == null) return NotFound();
            _context.RaidsCompleted.Remove(raidCompleted);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
