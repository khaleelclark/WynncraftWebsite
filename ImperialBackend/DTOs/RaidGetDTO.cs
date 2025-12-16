namespace ImperialBackend.DTOs
{
    public class RaidGetDTO
    {
        public int RaidId { get; set; }
        public string? RaidName { get; set; }
        public int SeasonRating { get; set; }
        public int CompletedCount { get; set; }
    }
}
