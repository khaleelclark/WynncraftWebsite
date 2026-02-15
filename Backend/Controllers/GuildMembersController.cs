using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend.Controllers;

[ApiController]
[Route("api/guildmembers")]
public class GuildMembersController : ControllerBase
{
    private readonly IGuildMemberService _service;

    public GuildMembersController(IGuildMemberService service)
    {
        _service = service;
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Post(GuildMemberPostDTO dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return MapInvalidOperation(ex);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Guild member not found", field = (string?)null });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, GuildMemberPostDTO dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Guild member not found", field = (string?)null });
        }
        catch (InvalidOperationException ex)
        {
            return MapInvalidOperation(ex);
        }
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetPublic() => Ok(await _service.GetAllPublicAsync());

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("admin")]
    public async Task<IActionResult> GetAdmin() => Ok(await _service.GetAllAdminAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            return Ok(await _service.GetByIdAsync(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Guild member not found", field = (string?)null });
        }
    }

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard(
        [FromQuery] DateTimeOffset startDate,
        [FromQuery] DateTimeOffset endDate
    ) => Ok(await _service.GetLeaderboardAsync(startDate, endDate));

    [Authorize(Policy = "AdminOnly")]
    [HttpGet()]
    public async Task<IActionResult> GetAllGeneric()
    {
        // Generic list used by admin dropdowns.
        return Ok(await _service.GetAllGenericAsync());
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Guild member not found", field = (string?)null });
        }
    }

    private IActionResult MapInvalidOperation(InvalidOperationException ex)
    {
        var msg = ex.Message;

        // Map validation errors to field names used by the admin UI.
        var field =
            msg.Contains("UUID", StringComparison.OrdinalIgnoreCase) ? "uuid"
            : msg.Contains("RankId", StringComparison.OrdinalIgnoreCase) ? "rank"
            : msg.Contains("MedalId", StringComparison.OrdinalIgnoreCase) ? "medals"
            : msg.Contains("GameId", StringComparison.OrdinalIgnoreCase) ? "games"
            : null;

        if (msg.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            return Conflict(new { message = msg, field });

        return BadRequest(new { message = msg, field });
    }
}
