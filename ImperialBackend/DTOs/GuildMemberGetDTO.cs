using System.ComponentModel.DataAnnotations;

namespace ImperialBackend.Models
{
    public class GuildMemberGetDTO
    {
        public int GuildMemberId { get; set; }
        public string? DiscordTag { get; set; }
        public string? MainUsername { get; set; }
        public string? MinecraftUsername { get; set; }
        public int RankId { get; set; }
        public DateOnly  JoinDate { get; set; }
        public Guid Uuid { get; set; }
        public string? WynncraftRank {get; set;}
        public int HoursPlayed { get; set; }
        public int WarsCompleted { get; set; }
        public int WeekliesCompleted { get; set; }
        public DateTimeOffset LastSynced { get; set; }
    }
}
