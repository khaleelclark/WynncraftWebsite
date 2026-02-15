namespace Backend.DTOs
{
    public class RaidCompletedPublicGetDTO
    {
        public int Id { get; set; }
        public DateTimeOffset CompletedDate { get; set; }
        public string RaidName { get; set; } = "";
        public List<string> GuildMembers { get; set; } = new();
    }
}
