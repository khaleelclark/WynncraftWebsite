using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/guildmembergames")]
    public class GuildMemberGamesController : ControllerBase
    {
        private readonly ImperialDbContext _context;
        public GuildMemberGamesController(ImperialDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.GuildMemberGames.ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var game = _context.GuildMemberGames.FirstOrDefault(g => g.GuildMemberGameId == id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        [HttpPost]
        public IActionResult Post([FromBody] GuildMemberGame game)
        {
            _context.GuildMemberGames.Add(game);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = game.GuildMemberGameId }, game);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GuildMemberGame game)
        {
            if (id != game.GuildMemberGameId) return BadRequest();
            _context.Entry(game).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var game = _context.GuildMemberGames.Find(id);
            if (game == null) return NotFound();
            _context.GuildMemberGames.Remove(game);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
