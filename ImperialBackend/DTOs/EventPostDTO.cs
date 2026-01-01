namespace ImperialBackend.Models
{
    public class EventPostDTO : GenericPostDTO
    {
        public DateTimeOffset EventStart { get; set; }
        public DateTimeOffset EventEnd { get; set; }
    }
}
