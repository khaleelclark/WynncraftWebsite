using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    public class EventsController : ODataController
    {
        private readonly ImperialDbContext _context;
        public EventsController(ImperialDbContext context) => _context = context;

        [EnableQuery]
        [HttpGet]
        public IQueryable<Event> Get() => _context.Events;

        [HttpPost]
        public IActionResult Post([FromBody] Event evt)
        {
            _context.Events.Add(evt);
            _context.SaveChanges();
            return Created(evt);
        }

        [HttpPut]
        [Route("odata/Events")]
        public IActionResult Put([FromBody] Event evt)
        {
            _context.Entry(evt).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(evt);
        }

        [HttpDelete]
        [Route("odata/Events")]
        public IActionResult Delete([FromBody] Event evt)
        {
            _context.Events.Remove(evt);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
