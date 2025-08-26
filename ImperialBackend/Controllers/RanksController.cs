using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
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
        public IActionResult GetAll() => Ok(_context.Ranks.ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var rank = _context.Ranks.Find(id);
            if (rank == null) return NotFound();
            return Ok(rank);
        }

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
