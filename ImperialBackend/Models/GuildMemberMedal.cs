using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ImperialBackend.Models
{
    public class GuildMemberMedal
    {
        [Key]
        public int GuildMemberMedalId { get; set; }
        public int MedalId { get; set; }
        public int GuildMemberId { get; set; }

        [JsonIgnore]
        public Medal Medal { get; set; } = null!;
        
        [JsonIgnore]
        public GuildMember GuildMember { get; set; } = null!;
    }
}
