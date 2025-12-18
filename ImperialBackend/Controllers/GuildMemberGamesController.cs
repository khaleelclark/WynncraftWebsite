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

        // [HttpPost]
        // public IActionResult Post([FromBody] GuildMemberGameDTO dto)
        // {
        //     var gameEntity = _context.Games.Find(dto.GameId);
        //     var memberEntity = _context.GuildMembers.Find(dto.GuildMemberId);
        //     if (gameEntity == null || memberEntity == null)
        //         return BadRequest(new { error = "Invalid GameId or GuildMemberId" });

        //     var guildMemberGame = new GuildMemberGame
        //     {
        //         GameId = dto.GameId,
        //         GuildMemberId = dto.GuildMemberId,
        //         Game = gameEntity,
        //         GuildMember = memberEntity
        //     };
        //     _context.GuildMemberGames.Add(guildMemberGame);
        //     _context.SaveChanges();
        //     var resultDto = new GuildMemberGameDTO
        //     {
        //         GameId = guildMemberGame.Game.GameId,
        //         GuildMemberId = guildMemberGame.GuildMember.GuildMemberId
        //     };

        //     return CreatedAtAction(nameof(GetById), new { id = guildMemberGame.GuildMemberGameId }, resultDto);
        // }
        [HttpPost]
        public IActionResult Post([FromBody] GuildMemberGameDTO dto)
        {
            if (!_context.Games.Any(g => g.GameId == dto.GameId))
                return BadRequest($"GameId {dto.GameId} does not exist.");

            if (!_context.GuildMembers.Any(m => m.GuildMemberId == dto.GuildMemberId))
                return BadRequest($"GuildMemberId {dto.GuildMemberId} does not exist.");

            bool exists = _context.GuildMemberGames.Any(x =>
                x.GameId == dto.GameId && x.GuildMemberId == dto.GuildMemberId);

            if (exists)
                return Conflict("That member already has that game.");

            var guildMemberGame = new GuildMemberGame
            {
                GameId = dto.GameId,
                GuildMemberId = dto.GuildMemberId
            };

            _context.GuildMemberGames.Add(guildMemberGame);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = guildMemberGame.GuildMemberGameId }, dto);
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
