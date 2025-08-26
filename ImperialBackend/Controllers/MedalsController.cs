using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    public class MedalsController : ODataController
    {
        private readonly ImperialDbContext _context;
        public MedalsController(ImperialDbContext context) => _context = context;

        [EnableQuery]
        [HttpGet]
        public IQueryable<Medal> Get() => _context.Medals.Include(m => m.GuildMemberMedals);

        [HttpPost]
        public IActionResult Post([FromBody] Medal medal)
        {
            _context.Medals.Add(medal);
            _context.SaveChanges();
            return Created(medal);
        }

        [HttpPut]
        [Route("odata/Medals")]
        public IActionResult Put([FromBody] Medal medal)
        {
            _context.Entry(medal).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(medal);
        }

        [HttpDelete]
        [Route("odata/Medals")]
        public IActionResult Delete([FromBody] Medal medal)
        {
            _context.Medals.Remove(medal);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
