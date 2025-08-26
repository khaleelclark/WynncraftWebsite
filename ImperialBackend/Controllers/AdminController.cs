using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly ImperialDbContext _context;
        public AdminController(ImperialDbContext context) => _context = context;

        // Example: Get database status
        [HttpGet("dbstatus")]
        public IActionResult GetDbStatus()
        {
            var canConnect = _context.Database.CanConnect();
            return Ok(new { DatabaseConnected = canConnect });
        }

        // Example: Get counts of main entities
        [HttpGet("counts")]
        public IActionResult GetCounts()
        {
            return Ok(new
            {
                GuildMembers = _context.GuildMembers.Count(),
                Games = _context.Games.Count(),
                Medals = _context.Medals.Count(),
                Raids = _context.Raids.Count(),
                Events = _context.Events.Count()
            });
        }
    }
}
