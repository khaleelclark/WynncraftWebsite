using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;
using ImperialBackend.DTOs;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/ranks")]
    public class RanksController : ControllerBase
    {
        private readonly ImperialDbContext _context;
        public RanksController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            var ranks = _context.Ranks
                .Select(r => new RankGetDTO
                {
                    RankId = r.RankId,
                    RankName = r.RankName,
                    MemberCount = r.GuildMembers.Count
                })
                .ToList();

            return Ok(ranks);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var rank = _context.Ranks
                .Where(r => r.RankId == id)
                .Select(r => new RankWithMembersDTO
                {
                    RankId = r.RankId,
                    RankName = r.RankName,
                    Members = r.GuildMembers.Select(m => new RankMemberDTO
                    {
                        GuildMemberId = m.GuildMemberId,
                        MainUsername = m.MainUsername,
                    }).ToList()
                })
                .FirstOrDefault();

            if (rank == null) return NotFound();
            return Ok(rank);
        }

// TODO: fix post and put for ranks
        [HttpPost]
        public IActionResult Post([FromBody] Rank rank)
        {
            _context.Ranks.Add(rank);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = rank.RankId }, rank);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Rank rank)
        {
            if (id != rank.RankId) return BadRequest();
            _context.Entry(rank).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var rank = _context.Ranks.Find(id);
            if (rank == null) return NotFound();
            _context.Ranks.Remove(rank);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
