using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    public class RanksController : ODataController
    {
        private readonly ImperialDbContext _context;
        public RanksController(ImperialDbContext context) => _context = context;

        [EnableQuery]
        [HttpGet]
        public IQueryable<Rank> Get() => _context.Ranks;

        [HttpPost]
        public IActionResult Post([FromBody] Rank rank)
        {
            _context.Ranks.Add(rank);
            _context.SaveChanges();
            return Created(rank);
        }

        [HttpPut]
        [Route("odata/Ranks")]
        public IActionResult Put([FromBody] Rank rank)
        {
            _context.Entry(rank).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(rank);
        }

        [HttpDelete]
        [Route("odata/Ranks")]
        public IActionResult Delete([FromBody] Rank rank)
        {
            _context.Ranks.Remove(rank);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
