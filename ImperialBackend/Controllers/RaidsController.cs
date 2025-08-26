using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    public class RaidsController : ODataController
    {
        private readonly ImperialDbContext _context;
        public RaidsController(ImperialDbContext context) => _context = context;

        [EnableQuery]
        [HttpGet]
        public IQueryable<Raid> Get() => _context.Raids.Include(r => r.RaidsCompleted);

        [HttpPost]
        public IActionResult Post([FromBody] Raid raid)
        {
            _context.Raids.Add(raid);
            _context.SaveChanges();
            return Created(raid);
        }

        [HttpPut]
        [Route("odata/Raids")]
        public IActionResult Put([FromBody] Raid raid)
        {
            _context.Entry(raid).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(raid);
        }

        [HttpDelete]
        [Route("odata/Raids")]
        public IActionResult Delete([FromBody] Raid raid)
        {
            _context.Raids.Remove(raid);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
