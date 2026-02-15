using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class GuildMemberGame
    {
        [Key]
        public int GuildMemberGameId { get; set; }
        public int GameId { get; set; }
        public int GuildMemberId { get; set; }

        public Game Game { get; set; } = null!;

        public GuildMember GuildMember { get; set; } = null!;
    }
}
