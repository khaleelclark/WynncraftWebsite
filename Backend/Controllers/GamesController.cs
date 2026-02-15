using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly IGameService _service;

    public GamesController(IGameService service)
    {
        _service = service;
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    [ResponseCache(Duration = 300)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Post(GenericPostDTO dto) =>
        Ok(await _service.CreateAsync(dto));

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, GenericPostDTO dto) =>
        Ok(await _service.UpdateAsync(id, dto));

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
