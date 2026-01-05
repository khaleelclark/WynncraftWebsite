using ImperialBackend.DTOs;

namespace ImperialBackend.Services
{
    public interface IRaidsCompletedService
    {
        Task<List<RaidCompletedGetDTO>> GetAllAsync();
        Task<RaidCompletedGetDTO> GetByIdAsync(int id);

        Task<List<RaidCompletedPublicGetDTO>> GetAllPublicAsync(
            DateTimeOffset? startDate,
            DateTimeOffset? endDate
        );

        Task<RaidCompletedGetDTO> UpdateAsync(int id, RaidCompletedPostDTO dto);
        Task<RaidCompletedGetDTO> CreateAsync(RaidCompletedPostDTO dto);

        Task DeleteAsync(int id);

        Task<object> SyncFromBotAsync(RaidBotReportDTO dto);
    }
}
