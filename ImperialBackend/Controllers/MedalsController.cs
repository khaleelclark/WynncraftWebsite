using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
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
        public IActionResult GetAll() => Ok(_context.Medals.Include(m => m.GuildMemberMedals).ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var medal = _context.Medals.Include(m => m.GuildMemberMedals).FirstOrDefault(m => m.MedalId == id);
            if (medal == null) return NotFound();
            return Ok(medal);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Medal medal)
        {
            _context.Medals.Add(medal);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = medal.MedalId }, medal);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Medal medal)
        {
            if (id != medal.MedalId) return BadRequest();
            _context.Entry(medal).State = EntityState.Modified;
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
