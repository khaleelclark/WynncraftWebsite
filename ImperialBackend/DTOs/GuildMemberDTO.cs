using System.ComponentModel.DataAnnotations;

namespace ImperialBackend.Models
{
    public class GuildMemberDTO
    {
        public string? DiscordTag { get; set; }
        public string? MainUsername { get; set; }
        public int RankId { get; set; }
        public DateTime JoinDate { get; set; }
        public Guid Uuid { get; set; }
    }
}
