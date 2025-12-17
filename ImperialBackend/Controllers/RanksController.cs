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

        //Change the rank of a guild member by guild member ID, takes in rank id in body
        [HttpPut("{id}/rank")]
        public IActionResult UpdateRank(int id, [FromBody] GuildMemberRankUpdateDTO dto)
        {
            var member = _context.GuildMembers.Find(id);
            if (member == null) return NotFound();

            var rankExists = _context.Ranks.Any(r => r.RankId == dto.RankId);
            if (!rankExists) return BadRequest($"RankId {dto.RankId} does not exist.");

            member.RankId = dto.RankId;
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
