namespace Backend.DTOs
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
        public List<RaidPartnerDTO> TopRaidPartners { get; set; } = new();
    }

    public class RaidPartnerDTO
    {
        public int GuildMemberId { get; set; }
        public Guid? Uuid { get; set; }
        public string? MainUsername { get; set; }
        public int TimesRaidedTogether { get; set; }
    }
}
