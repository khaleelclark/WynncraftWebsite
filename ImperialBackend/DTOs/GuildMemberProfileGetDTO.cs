namespace ImperialBackend.Models
{
    public class GuildMemberProfileGetDTO : GuildMemberAdminGetDTO
    {
        public int RaidsCompleted { get; set; }
        public List<string>? Games { get; set; }
        public List<string>? Medals { get; set; }
        public string? RankName { get; set; }
    }
}
