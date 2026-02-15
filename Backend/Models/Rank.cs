using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Rank
    {
        [Key]
        public int RankId { get; set; }
        public string? RankName { get; set; }
        public ICollection<GuildMember> GuildMembers { get; set; } = new List<GuildMember>();
    }
}
