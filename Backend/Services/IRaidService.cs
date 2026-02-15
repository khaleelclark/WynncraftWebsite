using Backend.DTOs;

public interface IRaidService
{
    Task<List<RaidGetDTO>> GetAllAsync();
    Task<RaidGetDTO> CreateAsync(RaidPostDTO dto);
    Task<RaidGetDTO> UpdateAsync(int id, RaidPostDTO dto);
    Task DeleteAsync(int id);
}
