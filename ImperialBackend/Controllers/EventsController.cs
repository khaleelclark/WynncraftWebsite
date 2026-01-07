using ImperialBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ImperialBackend.Controllers;

[ApiController]
[Route("api/events")]
[EnableRateLimiting("authenticated")]
public class EventsController : ControllerBase
{
    private readonly IEventService _service;

    public EventsController(IEventService service)
    {
        _service = service;
    }

    [HttpGet]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Post(EventPostDTO dto) => Ok(await _service.CreateAsync(dto));

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, EventPostDTO dto) =>
        Ok(await _service.UpdateAsync(id, dto));

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
