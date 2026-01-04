namespace ImperialBackend.DTOs
{
    public class GuildMemberProfileGetDTO : GuildMemberPublicGetDTO
    {
        public int RaidsCompleted { get; set; }
        public int HoursPlayed { get; set; }
        public int WarsCompleted { get; set; }
        public DateOnly JoinDate { get; set; }
        public DateTimeOffset? LastSynced { get; set; }
        public required List<string?> Games { get; set; }
        public required List<string?> Medals { get; set; }
    }
}
