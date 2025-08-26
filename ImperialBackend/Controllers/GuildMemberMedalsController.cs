using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    public class GuildMemberMedalsController : ODataController
    {
        private readonly ImperialDbContext _context;
        public GuildMemberMedalsController(ImperialDbContext context) => _context = context;

        [EnableQuery]
        [HttpGet]
        public IQueryable<GuildMemberMedal> Get() => _context.GuildMemberMedals.Include(gmm => gmm.Medal).Include(gmm => gmm.GuildMember);

        [HttpPost]
        public IActionResult Post([FromBody] GuildMemberMedal gmm)
        {
            _context.GuildMemberMedals.Add(gmm);
            _context.SaveChanges();
            return Created(gmm);
        }

        [HttpPut]
        public IActionResult Put([FromBody] GuildMemberMedal gmm)
        {
            _context.Entry(gmm).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(gmm);
        }

        [HttpDelete]
        [Route("odata/GuildMemberMedals")]
        public IActionResult Delete([FromBody] GuildMemberMedal gmm)
        {
            _context.GuildMemberMedals.Remove(gmm);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
