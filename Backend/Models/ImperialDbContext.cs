using Microsoft.EntityFrameworkCore;

namespace Backend.Models
{
    public class ImperialDbContext : DbContext
    {
        public ImperialDbContext(DbContextOptions<ImperialDbContext> options)
            : base(options) { }

        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Raid> Raids { get; set; }
        public DbSet<RaidCompleted> RaidsCompleted { get; set; }
        public DbSet<RaidInstance> RaidInstances { get; set; }
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

            modelBuilder
                .Entity<RaidInstance>()
                .HasOne(ri => ri.RaidCompleted)
                .WithMany(rc => rc.RaidInstances)
                .HasForeignKey(ri => ri.RaidCompletedId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<RaidInstance>()
                .HasOne(ri => ri.GuildMember)
                .WithMany()
                .HasForeignKey(ri => ri.GuildMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<RaidInstance>()
                .HasIndex(ri => new { ri.RaidCompletedId, ri.GuildMemberId })
                .IsUnique()
                .HasDatabaseName("UX_RaidInstances_RaidCompletedId_GuildMemberId");

            modelBuilder
                .Entity<GuildMember>()
                .HasIndex(gm => gm.RankId)
                .HasDatabaseName("IX_GuildMembers_RankId");

            modelBuilder
                .Entity<GuildMember>()
                .HasIndex(gm => gm.MinecraftUsername)
                .HasDatabaseName("IX_GuildMembers_MinecraftUsername");

            modelBuilder
                .Entity<GuildMember>()
                .HasIndex(gm => gm.LastSynced)
                .HasDatabaseName("IX_GuildMembers_LastSynced");

            modelBuilder
                .Entity<RaidCompleted>()
                .HasIndex(rc => rc.CompletedDate)
                .HasDatabaseName("IX_RaidsCompleted_CompletedDate");

            modelBuilder
                .Entity<RaidCompleted>()
                .HasIndex(rc => rc.RaidId)
                .HasDatabaseName("IX_RaidsCompleted_RaidId");

            modelBuilder
                .Entity<RaidCompleted>()
                .HasIndex(rc => new { rc.RaidId, rc.CompletedDate })
                .HasDatabaseName("IX_RaidsCompleted_RaidId_CompletedDate");

            modelBuilder
                .Entity<RaidInstance>()
                .HasIndex(ri => ri.GuildMemberId)
                .HasDatabaseName("IX_RaidInstances_GuildMemberId");

            modelBuilder
                .Entity<RaidInstance>()
                .HasIndex(ri => ri.RaidCompletedId)
                .HasDatabaseName("IX_RaidInstances_RaidCompletedId");

            modelBuilder
                .Entity<PlayerHistoricalStat>()
                .HasIndex(phs => new { phs.GuildMemberId, phs.SyncDate })
                .HasDatabaseName("IX_PlayerHistoricalStats_GuildMemberId_SyncDate");

            modelBuilder
                .Entity<PlayerHistoricalStat>()
                .HasIndex(phs => phs.GuildMemberId)
                .HasDatabaseName("IX_PlayerHistoricalStats_GuildMemberId");

            modelBuilder
                .Entity<Event>()
                .HasIndex(e => new { e.EventStart, e.EventEnd })
                .HasDatabaseName("IX_Events_EventStart_EventEnd");

            modelBuilder
                .Entity<Event>()
                .HasIndex(e => e.EventStart)
                .HasDatabaseName("IX_Events_EventStart");

            modelBuilder
                .Entity<Game>()
                .HasIndex(g => g.GameName)
                .HasDatabaseName("IX_Games_GameName");

            modelBuilder
                .Entity<GuildMemberGame>()
                .HasIndex(gmg => gmg.GameId)
                .HasDatabaseName("IX_GuildMemberGames_GameId");

            modelBuilder
                .Entity<GuildMemberGame>()
                .HasIndex(gmg => gmg.GuildMemberId)
                .HasDatabaseName("IX_GuildMemberGames_GuildMemberId");

            modelBuilder
                .Entity<GuildMemberMedal>()
                .HasIndex(gmm => gmm.MedalId)
                .HasDatabaseName("IX_GuildMemberMedals_MedalId");

            modelBuilder
                .Entity<GuildMemberMedal>()
                .HasIndex(gmm => gmm.GuildMemberId)
                .HasDatabaseName("IX_GuildMemberMedals_GuildMemberId");
        }
    }
}
