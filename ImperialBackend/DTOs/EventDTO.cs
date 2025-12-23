namespace ImperialBackend.Models
{
    public class EventDTO
    {
        public string? EventName { get; set; }
        public DateTimeOffset EventStart { get; set; }
        public DateTimeOffset EventEnd { get; set; }
    }
}