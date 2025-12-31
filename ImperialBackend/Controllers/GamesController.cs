using ImperialBackend.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetAll()
        {
            var games = _context
                .Games.AsNoTracking()
                .Select(g => new GenericGetDTO { Id = g.GameId, Name = g.GameName })
                .ToList();
            return Ok(games);
        }

        [HttpPost]
        public IActionResult Post([FromBody] GenericPostDTO game)
        {
            var existingGame = _context.Games.FirstOrDefault(g => g.GameName == game.Name);
            if (existingGame != null)
                return BadRequest("Game already exists");

            Game newGame = new Game { GameName = game.Name };

            _context.Games.Add(newGame);
            _context.SaveChanges();

            return Ok(newGame);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GenericPostDTO dto)
        {
            var game = _context.Games.Find(id);
            if (game == null)
                return BadRequest();

            game.GameName = dto.Name;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_context.GuildMemberGames.Any(g => g.GameId == id))
                return BadRequest(
                    "Cannot delete game with guild members. Remove guild members first"
                );

            var game = _context.Games.Find(id);
            if (game == null)
                return NotFound();
            _context.Games.Remove(game);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
