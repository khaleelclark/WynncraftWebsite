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
                SeasonRating = raid.SeasonRating
            };

            _context.Raids.Add(newRaid);
            _context.SaveChanges();

            return Ok(newRaid);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] RaidPostDTO raid)
        {
            var existingRaid = _context.Raids.Find(id);
            if (existingRaid == null) return NotFound();

            existingRaid.RaidName = raid.Name;
            existingRaid.SeasonRating = raid.SeasonRating;

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
