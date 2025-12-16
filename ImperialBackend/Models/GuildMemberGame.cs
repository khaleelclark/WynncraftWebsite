using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ImperialBackend.Models
{
    public class GuildMemberGame
    {
        [Key]
        public int GuildMemberGameId { get; set; }
        public int GameId { get; set; }
        public int GuildMemberId { get; set; }

        [JsonIgnore]
        public Game Game { get; set; } = null!;

        [JsonIgnore]
        public GuildMember GuildMember { get; set; } = null!;
    }
}
