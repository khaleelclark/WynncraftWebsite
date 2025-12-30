namespace ImperialBackend.Models
{
    public class GuildMemberPostDTO: GenericPostDTO
    {
        public string? DiscordTag { get; set; }
        public int RankId { get; set; }
        public DateOnly JoinDate { get; set; }
        public Guid Uuid { get; set; }
    }
}
