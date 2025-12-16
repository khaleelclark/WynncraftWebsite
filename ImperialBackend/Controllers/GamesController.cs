using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ControllerBase
    {
        private readonly ImperialDbContext _context;
        public GamesController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Games.Include(g => g.GuildMemberGames).ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var game = _context.Games.Include(g => g.GuildMemberGames).FirstOrDefault(g => g.GameId == id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        [HttpPost]
        public IActionResult Post([FromBody] GameDTO game)
        {
            Game newGame = new Game
            {
                GameName = game.GameName
            };

            _context.Games.Add(newGame);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = newGame.GameId }, game);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Game game)
        {
            if (id != game.GameId) return BadRequest();
            _context.Entry(game).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var game = _context.Games.Find(id);
            if (game == null) return NotFound();
            _context.Games.Remove(game);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
