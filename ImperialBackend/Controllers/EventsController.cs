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
        public IActionResult GetAll() {
            var events = _context.Events
                .Select(e => new EventGetDTO
                {
                    Id = e.EventId,
                    Name = e.EventName,
                    EventStart = e.EventStart,
                    EventEnd = e.EventEnd
                })
                .ToList();

            return Ok(events);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ev = _context.Events
                .Where(e => e.EventId == id)
                .Select(e => new EventGetDTO
                {
                    Id = e.EventId,
                    Name = e.EventName,
                    EventStart = e.EventStart,
                    EventEnd = e.EventEnd
                })
                .FirstOrDefault();

            if (ev == null)
                return NotFound();

            return Ok(ev);
        }

        [HttpPost]
        public IActionResult Post([FromBody] EventPostDTO ev)
        {
            Event newEvent = new Event
            {
                EventName = ev.Name,
                EventStart = ev.EventStart,
                EventEnd = ev.EventEnd
            };

            _context.Events.Add(newEvent);
            _context.SaveChanges();
            return Ok(new EventGetDTO
            {
                Id = newEvent.EventId,
                Name = newEvent.EventName,
                EventStart = newEvent.EventStart,
                EventEnd = newEvent.EventEnd
            });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] EventPostDTO ev)
        {
            var existingEvent = _context.Events.Find(id);
            if (existingEvent == null) return NotFound();

            existingEvent.EventName = ev.Name;
            existingEvent.EventStart = ev.EventStart;
            existingEvent.EventEnd = ev.EventEnd;

            _context.SaveChanges();
            return Ok(new EventGetDTO
            {
                Id = existingEvent.EventId,
                Name = existingEvent.EventName,
                EventStart = existingEvent.EventStart,
                EventEnd = existingEvent.EventEnd
            });
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
