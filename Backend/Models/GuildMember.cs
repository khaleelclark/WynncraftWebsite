using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class GuildMember
    {
        [Key]
        public int GuildMemberId { get; set; }
        public string? DiscordTag { get; set; }
        public string? MainUsername { get; set; }
        public string? MinecraftUsername { get; set; }
        public int RankId { get; set; }
        public DateOnly JoinDate { get; set; }
        public Guid? Uuid { get; set; }
        public string? WynncraftRank { get; set; }
        public int HoursPlayed { get; set; }
        public int WarsCompleted { get; set; }
        public int WeekliesCompleted { get; set; }
        public Rank? Rank { get; set; }
        public ICollection<GuildMemberGame> Games { get; set; } = new List<GuildMemberGame>();
        public ICollection<GuildMemberMedal> Medals { get; set; } = new List<GuildMemberMedal>();
        public ICollection<PlayerHistoricalStat> PlayerHistoricalStats { get; set; } =
            new List<PlayerHistoricalStat>();
        public DateTimeOffset? LastSynced { get; set; } // UTC timestamp of last sync
    }
}
