namespace ImperialBackend.Models
{
    public class GuildMemberProfileGetDTO : GuildMemberAdminGetDTO
    {
        public int RaidsCompleted { get; set; }
        public required List<string?> Games { get; set; }
        public required List<string?> Medals { get; set; }
        public string? RankName { get; set; }
    }
}
