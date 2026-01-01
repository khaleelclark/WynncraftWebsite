using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImperialBackend.Models
{
    public class Raid
    {
        [Key]
        public int RaidId { get; set; }
        public string? RaidName { get; set; }
        public int SeasonRating { get; set; }
        public ICollection<RaidCompleted> RaidsCompleted { get; set; } = new List<RaidCompleted>();
    }
}
