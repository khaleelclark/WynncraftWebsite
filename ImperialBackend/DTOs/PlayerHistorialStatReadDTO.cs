namespace ImperialBackend.Models
{
    public class PlayerHistoricalStatReadDTO
    {
        public int StatHistoryId { get; set; }
        public int WeekliesCompleted { get; set; }
        public int WarsCompleted { get; set; }
        public int HoursPlayed { get; set; }
        public DateTime SyncDate { get; set; }

        public int GuildMemberId { get; set; }
        public string? MinecraftUsername { get; set; }
        public string? MainUsername { get; set; }
        public string? DiscordTag { get; set; }
    }
}
