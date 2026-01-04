namespace ImperialBackend.DTOs
{
    public class RaidCompletedGetDTO
    {
        public int Id { get; set; } // RaidCompletedId
        public GenericGetDTO Raid { get; set; } = null!; // ✅ single object
        public DateTimeOffset CompletedDate { get; set; }

        public List<GenericGetDTO> GuildMembers { get; set; } = new(); // ✅ array
    }
}
