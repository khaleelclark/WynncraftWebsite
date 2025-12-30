namespace ImperialBackend.Models
{
    public class GuildMemberLeaderboardGetDTO : GenericGetDTO
    {

        public string MinecraftUsername { get; set; }
        public Guid Uuid { get; set; }
        public int HoursPlayed { get; set; }
        public int WarsCompleted { get; set; }
        public DateTimeOffset? LastSynced { get; set; }
        public int RaidsCompleted { get; set; }


    }
}
