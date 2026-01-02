using ImperialBackend.DTOs;
using ImperialBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var ranks = _context
                .Ranks.Select(r => new GenericGetDTO { Id = r.RankId, Name = r.RankName })
                .ToList();

            return Ok(ranks);
        }

        [HttpPost]
        public IActionResult Post([FromBody] GenericPostDTO dto)
        {
            var rank = new Rank { RankName = dto.Name };

            _context.Ranks.Add(rank);
            _context.SaveChanges();
            return Ok(new GenericGetDTO { Id = rank.RankId, Name = rank.RankName });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRank(int id, [FromBody] GenericPostDTO dto)
        {
            var rank = _context.Ranks.Find(id);
            if (rank == null)
                return NotFound();

            rank.RankName = dto.Name;
            _context.SaveChanges();

            return Ok(new GenericGetDTO { Id = rank.RankId, Name = rank.RankName });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var rank = _context
                .Ranks.Include(r => r.GuildMembers)
                .FirstOrDefault(r => r.RankId == id);

            if (rank == null)
                return NotFound();

            if (rank.GuildMembers.Any())
                return Conflict(
                    "Can't delete this rank because it still has guild members. Reassign them first."
                );

            _context.Ranks.Remove(rank);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
