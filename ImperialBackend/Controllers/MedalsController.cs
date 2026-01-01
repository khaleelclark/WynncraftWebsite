using ImperialBackend.DTOs;
using ImperialBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/medals")]
    public class MedalsController : ControllerBase
    {
        private readonly ImperialDbContext _context;

        public MedalsController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            var medals = _context
                .Medals.AsNoTracking()
                .Select(m => new GenericGetDTO { Id = m.MedalId, Name = m.MedalName })
                .ToList();
            return Ok(medals);
        }

        [HttpPost]
        public IActionResult Post([FromBody] GenericPostDTO medal)
        {
            Medal newMedal = new Medal { MedalName = medal.Name };

            _context.Medals.Add(newMedal);
            _context.SaveChanges();

            return Ok(newMedal);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GenericPostDTO medal)
        {
            var existingMedal = _context.Medals.Find(id);
            if (existingMedal == null)
                return NotFound();
            existingMedal.MedalName = medal.Name;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_context.GuildMemberMedals.Any(m => m.MedalId == id))
                return BadRequest(
                    "Cannot delete medal with guild members. Remove guild members first"
                );
            var medal = _context.Medals.Find(id);
            if (medal == null)
                return NotFound();
            _context.Medals.Remove(medal);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
