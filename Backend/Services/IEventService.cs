using Backend.DTOs;

public interface IEventService
{
    Task<List<EventGetDTO>> GetAllAsync();
    Task<EventGetDTO> GetByIdAsync(int id);
    Task<EventGetDTO> CreateAsync(EventPostDTO dto);
    Task<EventGetDTO> UpdateAsync(int id, EventPostDTO dto);
    Task DeleteAsync(int id);
}
