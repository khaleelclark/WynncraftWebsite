namespace ImperialBackend.Models
{
    public class GuildMemberPutDTO
    {
        public string? DiscordTag { get; set; }
        public string? MainUsername { get; set; }
        public int RankId { get; set; }
    }
}
