using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;
using ImperialBackend.DTOs;

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
            var raids = _context.Raids
                .Select(r => new RaidGetDTO
                {
                    RaidId = r.RaidId,
                    RaidName = r.RaidName,
                    SeasonRating = r.SeasonRaiting,
                    CompletedCount = r.RaidsCompleted.Count
                })
                .ToList();

            return Ok(raids);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var raid = _context.Raids
                .Where(r => r.RaidId == id)
                .Select(r => new RaidGetDTO
                {
                    RaidId = r.RaidId,
                    RaidName = r.RaidName,
                    SeasonRating = r.SeasonRaiting,
                    CompletedCount = r.RaidsCompleted.Count
                })
                .FirstOrDefault();

            if (raid == null) return NotFound();
            return Ok(raid);
        }


        [HttpPost]
        public IActionResult Post([FromBody] RaidPostDTO raid)
        {
            Raid newRaid = new Raid
            {
                RaidId = raid.RaidId,
                RaidName = raid.RaidName,
                SeasonRaiting = raid.SeasonRaiting
            };

            _context.Raids.Add(newRaid);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = raid.RaidId }, raid);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] RaidPostDTO raid)
        {
            var existingRaid = _context.Raids.Find(id);
            if (existingRaid == null) return NotFound();

            existingRaid.RaidName = raid.RaidName;
            existingRaid.SeasonRaiting = raid.SeasonRaiting;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var raid = _context.Raids.Find(id);
            if (raid == null) return NotFound();
            _context.Raids.Remove(raid);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
