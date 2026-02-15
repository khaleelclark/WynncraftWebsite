namespace Backend.DTOs
{
    public class RaidBotReportDTO
    {
        public int RaidId { get; set; }
        public DateTimeOffset? CompletedDate { get; set; }
        public List<string> MinecraftUsernames { get; set; } = new();
    }
}
