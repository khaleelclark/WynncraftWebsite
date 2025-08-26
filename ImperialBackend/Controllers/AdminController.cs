using Microsoft.AspNetCore.Mvc;
using ImperialBackend.Services;
using System.Threading.Tasks;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly GuildMemberSyncService _syncService;
        public AdminController(GuildMemberSyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost("run-nightly-sync")]
        public async Task<IActionResult> RunNightlySync()
        {
            await _syncService.RunNightlySyncManually();
            return Ok("Nightly sync completed.");
        }
    }
}
