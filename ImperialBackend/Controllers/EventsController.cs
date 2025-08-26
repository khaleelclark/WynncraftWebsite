using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly ImperialDbContext _context;
        public EventsController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Events.ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ev = _context.Events.FirstOrDefault(e => e.EventId == id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Event ev)
        {
            _context.Events.Add(ev);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = ev.EventId }, ev);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Event ev)
        {
            if (id != ev.EventId) return BadRequest();
            _context.Entry(ev).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var ev = _context.Events.Find(id);
            if (ev == null) return NotFound();
            _context.Events.Remove(ev);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
