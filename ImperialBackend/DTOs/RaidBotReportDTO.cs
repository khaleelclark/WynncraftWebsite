namespace ImperialBackend.DTOs
{
    public class RaidBotReportDTO
    {
        public int RaidId { get; set; }
        public DateTime? CompletedDate { get; set; }
        public List<string> MinecraftUsernames { get; set; } = new();
    }
}
