using System.ComponentModel.DataAnnotations;
using System;

namespace ImperialBackend.Models
{
    public class RaidCompleted
    {
        [Key]
        public int RaidCompletedId { get; set; }
        public int RaidId { get; set; }
        public int RaidInstanceId { get; set; }
        public Guid Uuid { get; set; }
        public DateTime CompletedDate { get; set; }
        public Raid Raid { get; set; } = null!;
        public GuildMember GuildMember { get; set; } = null!;
    }
}
