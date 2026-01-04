using ImperialBackend.DTOs;

public interface IGameService
{
    Task<List<GenericGetDTO>> GetAllAsync();
    Task<GenericGetDTO> CreateAsync(GenericPostDTO dto);
    Task<GenericGetDTO> UpdateAsync(int id, GenericPostDTO dto);
    Task DeleteAsync(int id);
}
