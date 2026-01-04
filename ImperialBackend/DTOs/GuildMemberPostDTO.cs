namespace ImperialBackend.DTOs
{
    public class GuildMemberPostDTO : GenericPostDTO
    {
        public string? DiscordTag { get; set; }
        public int Rank { get; set; }
        public DateOnly JoinDate { get; set; }
        public Guid Uuid { get; set; }
        public List<int>? Medals { get; set; }
        public List<int>? Games { get; set; }
    }
}
