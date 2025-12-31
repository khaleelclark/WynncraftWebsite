namespace ImperialBackend.Models
{
    public class GuildMemberAdminGetDTO: GuildMemberPublicGetDTO
    {
        public DateOnly JoinDate { get; set; }
        public int HoursPlayed { get; set; }
        public int WarsCompleted { get; set; }
        public int WeekliesCompleted { get; set; }
        public DateTimeOffset? LastSynced { get; set; }
    }
}
