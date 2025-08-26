using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    public class GuildMemberGamesController : ODataController
    {
        private readonly ImperialDbContext _context;
        public GuildMemberGamesController(ImperialDbContext context) => _context = context;

        [EnableQuery]
        [HttpGet]
        public IQueryable<GuildMemberGame> Get() => _context.GuildMemberGames.Include(gmg => gmg.Game).Include(gmg => gmg.GuildMember);

        [HttpPost]
        public IActionResult Post([FromBody] GuildMemberGame gmg)
        {
            _context.GuildMemberGames.Add(gmg);
            _context.SaveChanges();
            return Created(gmg);
        }

        [HttpPut]
        public IActionResult Put([FromBody] GuildMemberGame gmg)
        {
            _context.Entry(gmg).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(gmg);
        }

        [HttpDelete]
        [Route("odata/GuildMemberGames")]
        public IActionResult Delete([FromBody] GuildMemberGame gmg)
        {
            _context.GuildMemberGames.Remove(gmg);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
