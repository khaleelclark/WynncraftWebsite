using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    public class RaidsCompletedController : ODataController
    {
        private readonly ImperialDbContext _context;
        public RaidsCompletedController(ImperialDbContext context) => _context = context;

        [EnableQuery]
        [HttpGet]
        public IQueryable<RaidCompleted> Get() => _context.RaidsCompleted.Include(rc => rc.Raid);

        [HttpPost]
        public IActionResult Post([FromBody] RaidCompleted raidCompleted)
        {
            _context.RaidsCompleted.Add(raidCompleted);
            _context.SaveChanges();
            return Created(raidCompleted);
        }

        [HttpPut]
        public IActionResult Put([FromBody] RaidCompleted raidCompleted)
        {
            _context.Entry(raidCompleted).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(raidCompleted);
        }

        [HttpDelete]
        [Route("odata/RaidsCompleted")]
        public IActionResult Delete([FromBody] RaidCompleted raidCompleted)
        {
            _context.RaidsCompleted.Remove(raidCompleted);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
