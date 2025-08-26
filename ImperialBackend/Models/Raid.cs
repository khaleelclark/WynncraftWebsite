using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ImperialBackend.Models
{
    public class Raid
    {
        [Key]
        public int RaidId { get; set; }
        public string? RaidName { get; set; }
        public int SeasonRaiting { get; set; }
        public ICollection<RaidCompleted> RaidsCompleted { get; set; } = new List<RaidCompleted>();
    }
}
