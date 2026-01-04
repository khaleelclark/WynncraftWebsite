namespace ImperialBackend.DTOs
{
    public class EventGetDTO : GenericGetDTO
    {
        public DateTimeOffset EventStart { get; set; }
        public DateTimeOffset EventEnd { get; set; }
    }
}
