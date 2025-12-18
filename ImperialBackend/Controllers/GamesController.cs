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
        public IActionResult GetAll()
        {
            var games = _context.Games
                .AsNoTracking()
                .Select(g => new GameGetDTO
                {
                    GameId = g.GameId,
                    GameName = g.GameName,
                    MemberCount = g.GuildMemberGames.Count
                })
                .ToList();
            return Ok (games);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var game = _context.Games
                .AsNoTracking()
                .Where(g => g.GameId == id)
                .Select(g => new GameGetDTO
                {
                    GameId = g.GameId,
                    GameName = g.GameName,
                    MemberCount = g.GuildMemberGames.Count
                })
                .FirstOrDefault();

            if (game == null) return NotFound();
            return Ok(game);
        }

        //dont allow duplicate entries?
        [HttpPost]
        public IActionResult Post([FromBody] GameDTO game)
        {
            Game newGame = new Game
            {
                GameName = game.GameName
            };

            _context.Games.Add(newGame);
            _context.SaveChanges();
            var result = new GameGetDTO
            {
                GameId = newGame.GameId,
                GameName = newGame.GameName,
                MemberCount = 0
            };

            return CreatedAtAction(nameof(GetById), new { id = newGame.GameId }, result);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GameDTO dto)
        {
            var game = _context.Games.Find(id);
            if (game == null) return NotFound();

            game.GameName = dto.GameName;
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
