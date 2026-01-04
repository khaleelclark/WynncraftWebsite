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

            // ==============================================================
            // JUNCTION TABLE INDEXES (Already exist - good!)
            // ==============================================================
            modelBuilder
                .Entity<GuildMemberGame>()
                .HasIndex(gmg => new { gmg.GameId, gmg.GuildMemberId })
                .IsUnique();

            modelBuilder
                .Entity<GuildMemberMedal>()
                .HasIndex(gmm => new { gmm.MedalId, gmm.GuildMemberId })
                .IsUnique();

            // ==============================================================
            // CRITICAL INDEXES FOR QUERY PERFORMANCE
            // ==============================================================

            // GuildMember.Uuid - Used for:
            // - POST validation (FirstOrDefault by Uuid)
            // - RaidsCompleted lookups (Count by Uuid)
            // - Should be UNIQUE since it's a player identifier
            modelBuilder.Entity<GuildMember>().HasIndex(gm => gm.Uuid).IsUnique();

            // GuildMember.RankId - Foreign key queries and JOINs
            modelBuilder
                .Entity<GuildMember>()
                .HasIndex(gm => gm.RankId)
                .HasDatabaseName("IX_GuildMembers_RankId");

            // GuildMember.MinecraftUsername - Used in RaidsCompletedController for lookups
            // Query: FirstOrDefault(m => m.MinecraftUsername == username)
            modelBuilder
                .Entity<GuildMember>()
                .HasIndex(gm => gm.MinecraftUsername)
                .HasDatabaseName("IX_GuildMembers_MinecraftUsername");

            // GuildMember.LastSynced - For sync operations and filtering
            modelBuilder
                .Entity<GuildMember>()
                .HasIndex(gm => gm.LastSynced)
                .HasDatabaseName("IX_GuildMembers_LastSynced");

            // RaidsCompleted - Composite index for leaderboard queries
            // Query: WHERE CompletedDate >= @start AND CompletedDate <= @end
            // Then: GROUP BY Uuid
            modelBuilder
                .Entity<RaidCompleted>()
                .HasIndex(rc => new { rc.CompletedDate, rc.Uuid })
                .HasDatabaseName("IX_RaidsCompleted_CompletedDate_Uuid");

            // RaidsCompleted.Uuid - For individual player raid lookups
            // Query: Count(r => r.Uuid == member.Uuid)
            modelBuilder
                .Entity<RaidCompleted>()
                .HasIndex(rc => rc.Uuid)
                .HasDatabaseName("IX_RaidsCompleted_Uuid");

            // RaidsCompleted.RaidId - For filtering by raid type
            // Query: Count(rc => rc.RaidId == id)
            modelBuilder
                .Entity<RaidCompleted>()
                .HasIndex(rc => rc.RaidId)
                .HasDatabaseName("IX_RaidsCompleted_RaidId");

            // RaidsCompleted.RaidInstanceId - For Max queries and instance grouping
            // Query: Max(r => r.RaidInstanceId)
            modelBuilder
                .Entity<RaidCompleted>()
                .HasIndex(rc => rc.RaidInstanceId)
                .HasDatabaseName("IX_RaidsCompleted_RaidInstanceId");

            // RaidsCompleted - Composite for raid-specific queries with date filtering
            // Query: WHERE RaidId = @id AND CompletedDate >= @start AND CompletedDate <= @end
            modelBuilder
                .Entity<RaidCompleted>()
                .HasIndex(rc => new { rc.RaidId, rc.CompletedDate })
                .HasDatabaseName("IX_RaidsCompleted_RaidId_CompletedDate");

            // PlayerHistoricalStat - Composite for time-range queries
            // Query: WHERE GuildMemberId = @id AND SyncDate >= @start AND SyncDate <= @end
            // ORDER BY SyncDate
            modelBuilder
                .Entity<PlayerHistoricalStat>()
                .HasIndex(phs => new { phs.GuildMemberId, phs.SyncDate })
                .HasDatabaseName("IX_PlayerHistoricalStats_GuildMemberId_SyncDate");

            // PlayerHistoricalStat.GuildMemberId - For foreign key queries
            modelBuilder
                .Entity<PlayerHistoricalStat>()
                .HasIndex(phs => phs.GuildMemberId)
                .HasDatabaseName("IX_PlayerHistoricalStats_GuildMemberId");

            // Event.EventStart and EventEnd - For date range queries
            modelBuilder
                .Entity<Event>()
                .HasIndex(e => new { e.EventStart, e.EventEnd })
                .HasDatabaseName("IX_Events_EventStart_EventEnd");

            // Event.EventStart - For filtering events by start date
            modelBuilder
                .Entity<Event>()
                .HasIndex(e => e.EventStart)
                .HasDatabaseName("IX_Events_EventStart");

            // Game.GameName - For lookups by name
            // Query: FirstOrDefault(g => g.GameName == game.Name)
            modelBuilder
                .Entity<Game>()
                .HasIndex(g => g.GameName)
                .HasDatabaseName("IX_Games_GameName");

            // GuildMemberGame.GameId - For foreign key queries and filtering
            // Query: Any(g => g.GameId == id)
            modelBuilder
                .Entity<GuildMemberGame>()
                .HasIndex(gmg => gmg.GameId)
                .HasDatabaseName("IX_GuildMemberGames_GameId");

            // GuildMemberGame.GuildMemberId - For foreign key queries
            modelBuilder
                .Entity<GuildMemberGame>()
                .HasIndex(gmg => gmg.GuildMemberId)
                .HasDatabaseName("IX_GuildMemberGames_GuildMemberId");

            // GuildMemberMedal.MedalId - For foreign key queries and filtering
            // Query: Any(m => m.MedalId == id)
            modelBuilder
                .Entity<GuildMemberMedal>()
                .HasIndex(gmm => gmm.MedalId)
                .HasDatabaseName("IX_GuildMemberMedals_MedalId");

            // GuildMemberMedal.GuildMemberId - For foreign key queries
            modelBuilder
                .Entity<GuildMemberMedal>()
                .HasIndex(gmm => gmm.GuildMemberId)
                .HasDatabaseName("IX_GuildMemberMedals_GuildMemberId");
        }
    }
}
