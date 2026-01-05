using ImperialBackend.DTOs;
using ImperialBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImperialBackend.Controllers
{
    [ApiController]
    [Route("api/raidscompleted")]
    public class RaidsCompletedController : ControllerBase
    {
        private readonly IRaidsCompletedService _service;

        public RaidsCompletedController(IRaidsCompletedService service)
        {
            _service = service;
        }

        /* ============================
         * GET ALL
         * ============================ */

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var raids = await _service.GetAllAsync();
            return Ok(raids);
        }

        /* ============================
         * GET BY ID
         * ============================ */

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var raid = await _service.GetByIdAsync(id);
                return Ok(raid);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetAllPublic(
            [FromQuery] DateTimeOffset? startDate,
            [FromQuery] DateTimeOffset? endDate
        )
        {
            var raids = await _service.GetAllPublicAsync(startDate, endDate);
            return Ok(raids);
        }

        /* ============================
         * RAID BOT REPORT
         * ============================ */

        [HttpPost("raid-bot-report")]
        public async Task<IActionResult> SyncFromBot([FromBody] RaidBotReportDTO dto)
        {
            try
            {
                var result = await _service.SyncFromBotAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RaidCompletedPostDTO dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RaidCompletedPostDTO dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /* ============================
         * DELETE
         * ============================ */

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
