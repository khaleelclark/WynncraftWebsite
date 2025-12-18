namespace ImperialBackend.Models
{
    public class GuildMemberMedalGetDTO
    {
        public int GuildMemberMedalId { get; set; }
        public int MedalId { get; set; }
        public string? MedalName { get; set; }
        public int GuildMemberId { get; set; }
    }
}
