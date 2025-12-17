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

        // Creates a new rank. By default MemberCount is 0, guild members will have to be assigned to the rank separately.
        [HttpPost]
        public IActionResult Post([FromBody] RankPostDTO dto)
        {
            var rank = new Rank
            {
                RankName = dto.RankName
            };

            _context.Ranks.Add(rank);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = rank.RankId }, rank);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRank(int id, [FromBody] RankPostDTO dto)
        {
            var rank = _context.Ranks.Find(id);
            if (rank == null) return NotFound();

            rank.RankName = dto.RankName;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var rank = _context.Ranks
                .Include(r => r.GuildMembers)
                .FirstOrDefault(r => r.RankId == id);

            if (rank == null) return NotFound();

            if (rank.GuildMembers.Any())
                return Conflict("Can't delete this rank because it still has guild members. Reassign them first.");

            _context.Ranks.Remove(rank);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
