using ImperialBackend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ImperialBackend.Controllers;

[ApiController]
[Route("api/raids")]
public class RaidsController : ControllerBase
{
    private readonly IRaidService _service;

    public RaidsController(IRaidService service)
    {
        _service = service;
    }

    [HttpGet]
    [ResponseCache(Duration = 300)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Post(RaidPostDTO dto) => Ok(await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, RaidPostDTO dto) =>
        Ok(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
