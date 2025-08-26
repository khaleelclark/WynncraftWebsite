using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/raidscompleted")]
    public class RaidsCompletedController : ControllerBase
    {
        private readonly ImperialDbContext _context;
        public RaidsCompletedController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            var raids = _context.RaidsCompleted
                .Select(r => new
                {
                    r.RaidCompletedId,
                    r.RaidId,
                    r.RaidInstanceId,
                    r.Uuid,
                    r.CompletedDate
                })
                .ToList();
            return Ok(raids);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var raidCompleted = _context.RaidsCompleted.FirstOrDefault(r => r.RaidCompletedId == id);
            if (raidCompleted == null) return NotFound();
            return Ok(raidCompleted);
        }

        public class RaidsCompletedDto
        {
            public int RaidId { get; set; }
            public int RaidInstanceId { get; set; }
            public Guid Uuid { get; set; }
            public DateTime CompletedDate { get; set; }
        }

        [HttpPost]
        public IActionResult Post([FromBody] RaidsCompletedDto dto)
        {
            var raidCompleted = new RaidCompleted
            {
                RaidId = dto.RaidId,
                RaidInstanceId = dto.RaidInstanceId,
                Uuid = dto.Uuid,
                CompletedDate = dto.CompletedDate
            };
            _context.RaidsCompleted.Add(raidCompleted);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = raidCompleted.RaidCompletedId }, raidCompleted);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] RaidCompleted raidCompleted)
        {
            if (id != raidCompleted.RaidCompletedId) return BadRequest();
            _context.Entry(raidCompleted).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var raidCompleted = _context.RaidsCompleted.Find(id);
            if (raidCompleted == null) return NotFound();
            _context.RaidsCompleted.Remove(raidCompleted);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
