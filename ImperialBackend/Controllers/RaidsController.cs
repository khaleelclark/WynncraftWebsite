using ImperialBackend.DTOs;
using ImperialBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/raids")]
    public class RaidsController : ControllerBase
    {
        private readonly ImperialDbContext _context;

        public RaidsController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            var raids = _context
                .Raids.Select(r => new RaidGetDTO
                {
                    Id = r.RaidId,
                    Name = r.RaidName,
                    SeasonRating = r.SeasonRating,
                })
                .ToList();

            return Ok(raids);
        }

        [HttpPost]
        public IActionResult Post([FromBody] RaidPostDTO raid)
        {
            var newRaid = new Raid
            {
                RaidId = raid.Id,
                RaidName = raid.Name,
                SeasonRating = raid.SeasonRating,
            };

            _context.Raids.Add(newRaid);
            _context.SaveChanges();

            return Ok(
                new RaidGetDTO
                {
                    Id = newRaid.RaidId,
                    Name = newRaid.RaidName,
                    SeasonRating = newRaid.SeasonRating,
                }
            );
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] RaidPostDTO raid)
        {
            var existingRaid = _context.Raids.Find(id);
            if (existingRaid == null)
                return NotFound();

            existingRaid.RaidName = raid.Name;
            existingRaid.SeasonRating = raid.SeasonRating;

            _context.SaveChanges();
            return Ok(
                new RaidGetDTO
                {
                    Id = existingRaid.RaidId,
                    Name = existingRaid.RaidName,
                    SeasonRating = existingRaid.SeasonRating,
                }
            );
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var raid = _context.Raids.Find(id);
            if (raid == null)
                return NotFound();

            var refs = _context.RaidsCompleted.Count(rc => rc.RaidId == id);
            if (refs > 0)
                return Conflict(
                    $"Raid {id} can't be deleted because {refs} RaidsCompleted rows reference it."
                );

            _context.Raids.Remove(raid);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
