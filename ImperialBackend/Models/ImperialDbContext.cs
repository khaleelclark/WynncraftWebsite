using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Models
{
    public class ImperialDbContext : DbContext
    {
        public ImperialDbContext(DbContextOptions<ImperialDbContext> options)
            : base(options) { }

        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Raid> Raids { get; set; }
        public DbSet<RaidCompleted> RaidsCompleted { get; set; }
        public DbSet<PlayerHistoricalStat> PlayerHistoricalStats { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<GuildMember> GuildMembers { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GuildMemberGame> GuildMemberGames { get; set; }
        public DbSet<Medal> Medals { get; set; }
        public DbSet<GuildMemberMedal> GuildMemberMedals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<GuildMemberGame>()
                .HasIndex(gmg => new { gmg.GameId, gmg.GuildMemberId })
                .IsUnique();

            modelBuilder
                .Entity<GuildMemberMedal>()
                .HasIndex(gmm => new { gmm.MedalId, gmm.GuildMemberId })
                .IsUnique();
        }
    }
}
