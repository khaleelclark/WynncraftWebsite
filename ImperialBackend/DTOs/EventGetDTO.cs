namespace ImperialBackend.Models
{
    public class EventGetDTO: GenericGetDTO
    {
        public DateTimeOffset EventStart { get; set; }
        public DateTimeOffset EventEnd { get; set; }
    }
}