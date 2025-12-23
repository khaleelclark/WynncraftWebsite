using System.ComponentModel.DataAnnotations;
using System;

namespace ImperialBackend.Models
{
    public class PlayerHistoricalStat
    {
        [Key]
        public int StatHistoryId { get; set; }
        public int WeekliesCompleted { get; set; }
        public int WarsCompleted { get; set; }
        public int HoursPlayed { get; set; }
        public DateTimeOffset SyncDate { get; set; }
        public int GuildMemberId { get; set; }
        public GuildMember GuildMember { get; set; } = null!;
    }
}
