using ImperialBackend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ImperialBackend.Controllers;

[ApiController]
[Route("api/medals")]
public class MedalsController : ControllerBase
{
    private readonly IMedalService _service;

    public MedalsController(IMedalService service)
    {
        _service = service;
    }

    [HttpGet]
    [ResponseCache(Duration = 300)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Post(GenericPostDTO dto) =>
        Ok(await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, GenericPostDTO dto) =>
        Ok(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
