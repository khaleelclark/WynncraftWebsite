namespace ImperialBackend.DTOs
{
    public class RaidCompletedPostDTO
    {
        public int Raid { get; set; }
        public DateTimeOffset CompletedDate { get; set; }
        public List<int> GuildMembers { get; set; } = new();
    }
}
