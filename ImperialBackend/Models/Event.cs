using System.ComponentModel.DataAnnotations;
using System;

namespace ImperialBackend.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public int EventType { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }
    }
}
