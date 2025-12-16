namespace ImperialBackend.Models
{
    public class GuildMemberWriteDTO
    {
        public string? DiscordTag { get; set; }
        public string? MainUsername { get; set; }
        public int RankId { get; set; }
        public DateTime JoinDate { get; set; }
        public Guid Uuid { get; set; }
    }
}
