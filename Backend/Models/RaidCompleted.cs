using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class RaidCompleted
    {
        [Key]
        public int RaidCompletedId { get; set; }
        public int RaidId { get; set; }
        public DateTimeOffset CompletedDate { get; set; }
        public Raid? Raid { get; set; }
        public ICollection<RaidInstance> RaidInstances { get; set; } = new List<RaidInstance>();
    }
}
