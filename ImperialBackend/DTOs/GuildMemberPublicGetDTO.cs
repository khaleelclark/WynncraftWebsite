using System.ComponentModel.DataAnnotations;

namespace ImperialBackend.Models
{
    public class GuildMemberPublicGetDTO
    {
        public int GuildMemberId { get; set; }
        public string? DiscordTag { get; set; }
        public string? MainUsername { get; set; }
        public string? MinecraftUsername { get; set; }
        public int RankId { get; set; }
        public string? WynncraftRank {get; set;}
        public Guid Uuid { get; set; }
    }
}
