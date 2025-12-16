using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;


namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/guildmembermedals")]
    public class GuildMemberMedalsController : ControllerBase
    {
        private readonly ImperialDbContext _context;
        public GuildMemberMedalsController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.GuildMemberMedals.ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var medal = _context.GuildMemberMedals.FirstOrDefault(m => m.GuildMemberMedalId == id);
            if (medal == null) return NotFound();
            return Ok(medal);
        }

        [HttpPost]
        public IActionResult Post([FromBody] GuildMemberMedal medal)
        {
            GuildMemberMedal newMedal = new GuildMemberMedal
            {
                MedalId = medal.MedalId,
                GuildMemberId = medal.GuildMemberId
            };

            _context.GuildMemberMedals.Add(medal);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = medal.GuildMemberMedalId }, medal);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GuildMemberMedal medal)
        {
            if (id != medal.GuildMemberMedalId) return BadRequest();
            _context.Entry(medal).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var medal = _context.GuildMemberMedals.Find(id);
            if (medal == null) return NotFound();
            _context.GuildMemberMedals.Remove(medal);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
