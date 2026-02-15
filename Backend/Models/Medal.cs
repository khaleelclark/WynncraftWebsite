using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Medal
    {
        [Key]
        public int MedalId { get; set; }
        public string? MedalName { get; set; }
        public ICollection<GuildMemberMedal> GuildMemberMedals { get; set; } =
            new List<GuildMemberMedal>();
    }
}
