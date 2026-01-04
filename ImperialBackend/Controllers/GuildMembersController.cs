using ImperialBackend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ImperialBackend.Controllers;

[ApiController]
[Route("api/guildmembers")]
public class GuildMembersController : ControllerBase
{
    private readonly IGuildMemberService _service;

    public GuildMembersController(IGuildMemberService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Post(GuildMemberPostDTO dto) =>
        Ok(await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, GuildMemberPostDTO dto) =>
        Ok(await _service.UpdateAsync(id, dto));

    [HttpGet]
    public async Task<IActionResult> GetPublic() => Ok(await _service.GetAllPublicAsync());

    [HttpGet("admin")]
    public async Task<IActionResult> GetAdmin() => Ok(await _service.GetAllAdminAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
