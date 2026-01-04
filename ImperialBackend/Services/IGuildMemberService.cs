using ImperialBackend.DTOs;

public interface IGuildMemberService
{
    Task<GuildMemberAdminGetDTO> CreateAsync(GuildMemberPostDTO dto);
    Task<GuildMemberAdminGetDTO> UpdateAsync(int id, GuildMemberPostDTO dto);
    Task<List<GuildMemberPublicGetDTO>> GetAllPublicAsync();
    Task<List<GuildMemberAdminGetDTO>> GetAllAdminAsync();
    Task<GuildMemberProfileGetDTO> GetByIdAsync(int id);
    Task<List<GenericGetDTO>> GetAllGenericAsync();

    Task<List<GuildMemberLeaderboardGetDTO>> GetLeaderboardAsync(
        DateTimeOffset startDate,
        DateTimeOffset endDate
    );
    Task DeleteAsync(int id);
}
