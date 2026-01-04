using System.ComponentModel.DataAnnotations;

namespace ImperialBackend.Models
{
    public class RaidInstance
    {
        [Key]
        public int RaidInstanceId { get; set; }
        public int RaidCompletedId { get; set; }
        public RaidCompleted? RaidCompleted { get; set; }
        public GuildMember? GuildMember { get; set; }
        public int GuildMemberId { get; set; }
    }
}
