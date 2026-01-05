namespace ImperialBackend.DTOs
{
    public class RaidCompletedGetDTO
    {
        public int Id { get; set; }
        public GenericGetDTO Raid { get; set; } = null!;
        public DateTimeOffset CompletedDate { get; set; }

        public List<GenericGetDTO> GuildMembers { get; set; } = new();
    }
}
