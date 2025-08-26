using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ImperialBackend.Models
{
    public class Medal
    {
        [Key]
        public int MedalId { get; set; }
        public string? MedalName { get; set; }
        public ICollection<GuildMemberMedal> GuildMemberMedals { get; set; } = new List<GuildMemberMedal>();
    }
}
