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
        public IActionResult GetAll()
        {
            var medals = _context.GuildMemberMedals
                .Include(m => m.Medal)
                .Select(m => new GuildMemberMedalGetDTO
                {
                    GuildMemberMedalId = m.GuildMemberMedalId,
                    MedalId = m.MedalId,
                    MedalName = m.Medal.MedalName,
                    GuildMemberId = m.GuildMemberId
                })
                .ToList();

            return Ok(medals);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var medal = _context.GuildMemberMedals
                .Include(m => m.Medal)
                .Where(m => m.GuildMemberMedalId == id)
                .Select(m => new GuildMemberMedalGetDTO
                {
                    GuildMemberMedalId = m.GuildMemberMedalId,
                    MedalId = m.MedalId,
                    MedalName = m.Medal.MedalName,
                    GuildMemberId = m.GuildMemberId
                })
                .FirstOrDefault();

            if (medal == null) return NotFound();
            return Ok(medal);
        }

        [HttpPost]
        public IActionResult Post([FromBody] GuildMemberMedalPostDTO medal)
        {

            if (!_context.Medals.Any(m => m.MedalId == medal.MedalId))
                return BadRequest($"MedalId {medal.MedalId} does not exist.");

            if (!_context.GuildMembers.Any(g => g.GuildMemberId == medal.GuildMemberId))
                return BadRequest($"GuildMemberId {medal.GuildMemberId} does not exist.");

            //prevent duplicates
            bool exists = _context.GuildMemberMedals.Any(x =>
                x.GuildMemberId == medal.GuildMemberId && x.MedalId == medal.MedalId);

            if (exists)
                return Conflict("That member already has that medal.");

            GuildMemberMedal newMedal = new GuildMemberMedal
            {
                MedalId = medal.MedalId,
                GuildMemberId = medal.GuildMemberId
            };

            _context.GuildMemberMedals.Add(newMedal);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), 
            new { id = newMedal.GuildMemberMedalId },
            new { newMedal.GuildMemberMedalId, newMedal.GuildMemberId, newMedal.MedalId });
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
