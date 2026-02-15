namespace Backend.DTOs
{
    public class EventPostDTO : GenericPostDTO
    {
        public DateTimeOffset EventStart { get; set; }
        public DateTimeOffset EventEnd { get; set; }
    }
}
