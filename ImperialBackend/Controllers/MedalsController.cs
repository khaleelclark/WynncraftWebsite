using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;
using ImperialBackend.DTOs;

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
            var medals = _context.Medals
                .AsNoTracking()
                .Select(m => new
                {
                    m.MedalId,
                    m.MedalName,
                    MemberCount = m.GuildMemberMedals.Count
                })
                .ToList();
            return Ok(medals);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var medal = _context.Medals
                .AsNoTracking()
                .Where(m => m.MedalId == id)
                .Select(m => new
                {
                    m.MedalId,
                    m.MedalName,
                    MemberCount = m.GuildMemberMedals.Count
                })
                .FirstOrDefault();

            if (medal == null) return NotFound();
            return Ok(medal);
        }

        [HttpPost]
        public IActionResult Post([FromBody] MedalDTO medal)
        {
            Medal newMedal = new Medal
            {
                MedalName = medal.MedalName
            };

            _context.Medals.Add(newMedal);
            _context.SaveChanges();
            var result = new MedalGetDTO
            {
                MedalId = newMedal.MedalId,
                MedalName = newMedal.MedalName,
                MemberCount = 0
            };

            return CreatedAtAction(nameof(GetById), new { id = newMedal.MedalId }, result);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] MedalDTO medal)
        {

            if (string.IsNullOrWhiteSpace(medal.MedalName))
                return BadRequest("MedalName is required.");

            var existingMedal = _context.Medals.Find(id);
            if (existingMedal == null) return NotFound();
            existingMedal.MedalName = medal.MedalName;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var medal = _context.Medals.Find(id);
            if (medal == null) return NotFound();
            _context.Medals.Remove(medal);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
