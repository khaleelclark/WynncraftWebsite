using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class GuildMemberMedal
    {
        [Key]
        public int GuildMemberMedalId { get; set; }
        public int MedalId { get; set; }
        public int GuildMemberId { get; set; }

        public Medal Medal { get; set; } = null!;

        public GuildMember GuildMember { get; set; } = null!;
    }
}
