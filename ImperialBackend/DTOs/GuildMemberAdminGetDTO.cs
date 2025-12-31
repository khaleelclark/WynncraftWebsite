namespace ImperialBackend.Models
{
    public class GuildMemberAdminGetDTO: GenericGetDTO
    {
        public string? DiscordTag { get; set; }
        public string? MinecraftUsername { get; set; }
        public string? WynncraftRank { get; set;}
        public Guid Uuid { get; set; }
        public DateOnly JoinDate { get; set; }
        public GenericGetDTO? Rank { get; set; }
        public required List<GenericGetDTO> Games { get; set; }
        public required List<GenericGetDTO> Medals { get; set; }
    }
}
