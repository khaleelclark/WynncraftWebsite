using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/playerhistoricalstats")]
    public class PlayerHistoricalStatsController : ControllerBase
    {
        private readonly ImperialDbContext _context;
        public PlayerHistoricalStatsController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.PlayerHistoricalStats.ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var stat = _context.PlayerHistoricalStats.FirstOrDefault(s => s.StatHistoryId == id);
            if (stat == null) return NotFound();
            return Ok(stat);
        }

        [HttpPost]
        public IActionResult Post([FromBody] PlayerHistoricalStat stat)
        {
            _context.PlayerHistoricalStats.Add(stat);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = stat.StatHistoryId }, stat);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] PlayerHistoricalStat stat)
        {
            if (id != stat.StatHistoryId) return BadRequest();
            _context.Entry(stat).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var stat = _context.PlayerHistoricalStats.Find(id);
            if (stat == null) return NotFound();
            _context.PlayerHistoricalStats.Remove(stat);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
