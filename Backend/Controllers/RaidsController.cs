using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend.Controllers;

[ApiController]
[Route("api/raids")]
public class RaidsController : ControllerBase
{
    private readonly IRaidService _service;

    public RaidsController(IRaidService service)
    {
        _service = service;
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    [ResponseCache(Duration = 300)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Post(RaidPostDTO dto)
    {
        try
        {
            return Ok(await _service.CreateAsync(dto));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, RaidPostDTO dto)
    {
        try
        {
            return Ok(await _service.UpdateAsync(id, dto));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Raid {id} was not found." });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
