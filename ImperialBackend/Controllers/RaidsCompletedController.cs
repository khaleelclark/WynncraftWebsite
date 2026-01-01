using ImperialBackend.DTOs;
using ImperialBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    // add a put for admin panel

    [ApiController]
    [Route("api/raidscompleted")]
    public class RaidsCompletedController : ControllerBase
    {
        private readonly ImperialDbContext _context;

        public RaidsCompletedController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            var raids = _context
                .RaidsCompleted.Select(r => new
                {
                    r.RaidCompletedId,
                    r.RaidId,
                    r.RaidInstanceId,
                    r.Uuid,
                    r.CompletedDate,
                })
                .ToList();
            return Ok(raids);
        }

        // [HttpPut("{id}")]
        // public IActionResult UpdateRank(int id, [FromBody] RaidsComplDTO dto)
        // {
        //     var rank = _context.Ranks.Find(id);
        //     if (rank == null) return NotFound();

        //     rank.RankName = dto.RankName;
        //     _context.SaveChanges();

        //     return NoContent();
        // }

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
                (
                    _context.RaidsCompleted.Any()
                        ? _context.RaidsCompleted.Max(r => r.RaidInstanceId)
                        : 0
                ) + 1;

            var created = new List<object>();
            var notFoundUsers = new List<string>();

            foreach (
                var username in dto.MinecraftUsernames.Distinct(StringComparer.OrdinalIgnoreCase)
            )
            {
                var member = _context.GuildMembers.FirstOrDefault(m =>
                    m.MinecraftUsername == username
                );

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
                    GuildMember = member,
                };

                _context.RaidsCompleted.Add(rc);

                created.Add(
                    new
                    {
                        username,
                        member.Uuid,
                        raid.RaidName,
                        newRaidInstanceId,
                        completedDateUtc,
                    }
                );
            }

            _context.SaveChanges();

            return Ok(
                new
                {
                    raidId = raid.RaidId,
                    raidName = raid.RaidName,
                    raidInstanceId = newRaidInstanceId,
                    completedDateUtc,
                    createdCount = created.Count,
                    notFoundUsers,
                }
            );
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var raidCompleted = _context.RaidsCompleted.Find(id);
            if (raidCompleted == null)
                return NotFound();
            _context.RaidsCompleted.Remove(raidCompleted);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
