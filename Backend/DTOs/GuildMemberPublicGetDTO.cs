namespace Backend.DTOs
{
    public class GuildMemberPublicGetDTO : GenericGetDTO
    {
        public string? DiscordTag { get; set; }
        public string? MinecraftUsername { get; set; }
        public string? RankName { get; set; }
        public string? WynncraftRank { get; set; }
        public Guid? Uuid { get; set; }
    }
}
