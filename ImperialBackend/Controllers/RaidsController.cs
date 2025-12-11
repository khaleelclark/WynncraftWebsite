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
        public IActionResult GetAll() => Ok(_context.Raids.Include(r => r.RaidsCompleted).ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var raid = _context.Raids.Include(r => r.RaidsCompleted).FirstOrDefault(r => r.RaidId == id);
            if (raid == null) return NotFound();
            return Ok(raid);
        }

        [HttpPost]
        public IActionResult Post([FromBody] RaidDTO raid)
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
        public IActionResult Put(int id, [FromBody] Raid raid)
        {
            if (id != raid.RaidId) return BadRequest();
            _context.Entry(raid).State = EntityState.Modified;
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
