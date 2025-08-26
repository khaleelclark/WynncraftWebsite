using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    public class GamesController : ODataController
    {
        private readonly ImperialDbContext _context;
        public GamesController(ImperialDbContext context) => _context = context;

        [EnableQuery]
        [HttpGet]
        public IQueryable<Game> Get() => _context.Games.Include(g => g.GuildMemberGames);

        [HttpPost]
        public IActionResult Post([FromBody] Game game)
        {
            _context.Games.Add(game);
            _context.SaveChanges();
            return Created(game);
        }

        [HttpPut]
        [Route("odata/Games")]
        public IActionResult Put([FromBody] Game game)
        {
            _context.Entry(game).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(game);
        }

        [HttpDelete]
        [Route("odata/Games")]
        public IActionResult Delete([FromBody] Game game)
        {
            _context.Games.Remove(game);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
