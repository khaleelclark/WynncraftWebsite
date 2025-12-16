using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
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
        //absolutley not
        public IActionResult Post([FromBody] GuildMemberGameDTO dto)
        {
            var gameEntity = _context.Games.Find(dto.GameId);
            var memberEntity = _context.GuildMembers.Find(dto.GuildMemberId);
            if (gameEntity == null || memberEntity == null)
                return BadRequest(new { error = "Invalid GameId or GuildMemberId" });

            var guildMemberGame = new GuildMemberGame
            {
                GameId = dto.GameId,
                GuildMemberId = dto.GuildMemberId,
                Game = gameEntity,
                GuildMember = memberEntity
            };
            _context.GuildMemberGames.Add(guildMemberGame);
            _context.SaveChanges();
            var resultDto = new GuildMemberGameDTO
            {
                GameId = guildMemberGame.Game.GameId,
                GuildMemberId = guildMemberGame.GuildMember.GuildMemberId
            };

            return CreatedAtAction(nameof(GetById), new { id = guildMemberGame.GuildMemberGameId }, resultDto);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] GuildMemberGameDTO dto)
        {
            var existing = _context.GuildMemberGames.Find(id);
            if (existing == null) return NotFound();

            var gameEntity = _context.Games.Find(dto.GameId);
            var memberEntity = _context.GuildMembers.Find(dto.GuildMemberId);
            if (gameEntity == null || memberEntity == null)
                return BadRequest(new { error = "Invalid GameId or GuildMemberId" });

            existing.GameId = dto.GameId;
            existing.GuildMemberId = dto.GuildMemberId;
            existing.Game = gameEntity;
            existing.GuildMember = memberEntity;
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
