using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend.Controllers
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

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var raids = await _service.GetAllAsync();
            return Ok(raids);
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetAllPublic(
            [FromQuery] DateTimeOffset? startDate,
            [FromQuery] DateTimeOffset? endDate
        )
        {
            // Public endpoint supports optional date range filtering.
            var raids = await _service.GetAllPublicAsync(startDate, endDate);
            return Ok(raids);
        }

        [Authorize(Policy = "AdminOnly")]
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

        [Authorize(Policy = "AdminOnly")]
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

        [Authorize(Policy = "AdminOnly")]
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
