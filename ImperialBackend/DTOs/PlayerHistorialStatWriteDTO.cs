namespace ImperialBackend.Models
{
    public class PlayerHistoricalStatWriteDTO
    {
        public int WeekliesCompleted { get; set; }
        public int WarsCompleted { get; set; }
        public int HoursPlayed { get; set; }
        public DateTimeOffset SyncDate { get; set; }

        public int GuildMemberId { get; set; }
    }
}
