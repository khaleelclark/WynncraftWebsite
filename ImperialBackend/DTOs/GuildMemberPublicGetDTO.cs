namespace ImperialBackend.Models
{
    public class GuildMemberPublicGetDTO: GenericGetDTO
    {
        public string? DiscordTag { get; set; }
        public string? MinecraftUsername { get; set; }
        //public GenericGetDTO Rank { get; set; }
        public int RankId { get; set;}
        public string? WynncraftRank { get; set;}
        public Guid Uuid { get; set; }
    }
}
