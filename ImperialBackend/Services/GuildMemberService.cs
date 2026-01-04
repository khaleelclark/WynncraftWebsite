using ImperialBackend.DTOs;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Services
{
    public class GuildMemberService : IGuildMemberService
    {
        private readonly ImperialDbContext _context;

        public GuildMemberService(ImperialDbContext context)
        {
            _context = context;
        }

        /* ============================
         * CREATE
         * ============================ */

        public async Task<GuildMemberAdminGetDTO> CreateAsync(GuildMemberPostDTO dto)
        {
            if (await _context.GuildMembers.AnyAsync(m => m.Uuid == dto.Uuid))
                throw new InvalidOperationException("Guild member with this UUID already exists");

            if (!await _context.Ranks.AnyAsync(r => r.RankId == dto.Rank))
                throw new InvalidOperationException("Invalid RankId");

            await ValidateIdsAsync(dto);

            var member = new GuildMember
            {
                MainUsername = dto.Name,
                DiscordTag = dto.DiscordTag,
                JoinDate = dto.JoinDate,
                Uuid = dto.Uuid,
                RankId = dto.Rank,
                Medals =
                    dto.Medals?.Distinct()
                        .Select(id => new GuildMemberMedal { MedalId = id })
                        .ToList()
                    ?? new(),
                Games =
                    dto.Games?.Distinct().Select(id => new GuildMemberGame { GameId = id }).ToList()
                    ?? new(),
            };

            _context.GuildMembers.Add(member);
            await _context.SaveChangesAsync();

            return await LoadAdminDtoAsync(member.GuildMemberId);
        }

        /* ============================
         * UPDATE
         * ============================ */

        public async Task<GuildMemberAdminGetDTO> UpdateAsync(int id, GuildMemberPostDTO dto)
        {
            await ValidateIdsAsync(dto);

            await using var tx = await _context.Database.BeginTransactionAsync();

            var member = await _context
                .GuildMembers.Include(m => m.Medals)
                .Include(m => m.Games)
                .FirstOrDefaultAsync(m => m.GuildMemberId == id);

            if (member == null)
                throw new KeyNotFoundException();

            member.MainUsername = dto.Name;
            member.DiscordTag = dto.DiscordTag;
            member.JoinDate = dto.JoinDate;
            member.RankId = dto.Rank;
            member.Uuid = dto.Uuid;

            SyncCollection(
                member.Medals,
                dto.Medals,
                m => m.MedalId,
                medalId => new GuildMemberMedal { MedalId = medalId }
            );

            SyncCollection(
                member.Games,
                dto.Games,
                g => g.GameId,
                gameId => new GuildMemberGame { GameId = gameId }
            );

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return await LoadAdminDtoAsync(id);
        }

        /* ============================
         * GET ALL (PUBLIC)
         * ============================ */

        public async Task<List<GuildMemberPublicGetDTO>> GetAllPublicAsync()
        {
            return await _context
                .GuildMembers.AsNoTracking()
                .Include(m => m.Rank)
                .Select(m => new GuildMemberPublicGetDTO
                {
                    Id = m.GuildMemberId,
                    Name = m.MainUsername,
                    DiscordTag = m.DiscordTag,
                    MinecraftUsername = m.MinecraftUsername,
                    RankName = m.Rank!.RankName,
                    WynncraftRank = m.WynncraftRank,
                    Uuid = m.Uuid,
                })
                .ToListAsync();
        }

        /* ============================
         * GET ALL (PUBLIC - GENERIC LIST)
         * ============================ */

        public async Task<List<GenericGetDTO>> GetAllGenericAsync()
        {
            return await _context
                .GuildMembers.AsNoTracking()
                .OrderBy(m => m.MainUsername)
                .Select(m => new GenericGetDTO { Id = m.GuildMemberId, Name = m.MainUsername })
                .ToListAsync();
        }

        /* ============================
         * GET ALL (ADMIN)
         * ============================ */

        public async Task<List<GuildMemberAdminGetDTO>> GetAllAdminAsync()
        {
            var members = await _context
                .GuildMembers.AsNoTracking()
                .Include(m => m.Rank)
                .Include(m => m.Games)
                    .ThenInclude(g => g.Game)
                .Include(m => m.Medals)
                    .ThenInclude(m => m.Medal)
                .ToListAsync();

            return members.Select(ToAdminDto).ToList();
        }

        /* ============================
         * GET BY ID (PROFILE)
         * ============================ */

        public async Task<GuildMemberProfileGetDTO> GetByIdAsync(int id)
        {
            var member = await _context
                .GuildMembers.AsNoTracking()
                .Include(m => m.PlayerHistoricalStats)
                .Include(m => m.Games)
                    .ThenInclude(g => g.Game)
                .Include(m => m.Medals)
                    .ThenInclude(m => m.Medal)
                .Include(m => m.Rank)
                .FirstOrDefaultAsync(m => m.GuildMemberId == id);

            if (member == null)
                throw new KeyNotFoundException();

            var oldest = member.PlayerHistoricalStats.OrderBy(s => s.SyncDate).FirstOrDefault();

            return new GuildMemberProfileGetDTO
            {
                Id = member.GuildMemberId,
                Name = member.MainUsername,
                DiscordTag = member.DiscordTag,
                JoinDate = member.JoinDate,
                MinecraftUsername = member.MinecraftUsername,
                WynncraftRank = member.WynncraftRank,
                Uuid = member.Uuid,
                RankName = member.Rank!.RankName,
                WarsCompleted = oldest == null ? 0 : member.WarsCompleted - oldest.WarsCompleted,
                HoursPlayed = oldest == null ? 0 : member.HoursPlayed - oldest.HoursPlayed,
                RaidsCompleted = await _context.RaidInstances.CountAsync(ri =>
                    ri.GuildMemberId == member.GuildMemberId
                ),
                LastSynced = member.LastSynced,
                Games = member.Games.Select(g => g.Game!.GameName).ToList(),
                Medals = member.Medals.Select(m => m.Medal!.MedalName).ToList(),
            };
        }

        /* ============================
         * LEADERBOARD
         * ============================ */

        public async Task<List<GuildMemberLeaderboardGetDTO>> GetLeaderboardAsync(
            DateTimeOffset startDate,
            DateTimeOffset endDate
        )
        {
            var members = await _context
                .GuildMembers.Include(m => m.PlayerHistoricalStats)
                .ToListAsync();

            var raidCounts = await _context
                .RaidInstances.Where(ri =>
                    ri.RaidCompleted != null
                    && ri.RaidCompleted.CompletedDate >= startDate
                    && ri.RaidCompleted.CompletedDate <= endDate
                )
                .GroupBy(ri => ri.GuildMemberId)
                .Select(g => new { GuildMemberId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.GuildMemberId, x => x.Count);

            var leaderboard = new List<GuildMemberLeaderboardGetDTO>();

            foreach (var m in members)
            {
                var stats = m
                    .PlayerHistoricalStats.Where(s =>
                        s.SyncDate >= startDate && s.SyncDate <= endDate
                    )
                    .OrderBy(s => s.SyncDate)
                    .ToList();

                stats.Add(
                    new PlayerHistoricalStat
                    {
                        WarsCompleted = m.WarsCompleted,
                        HoursPlayed = m.HoursPlayed,
                        SyncDate = DateTimeOffset.UtcNow,
                    }
                );

                int wars = 0,
                    hours = 0;

                if (stats.Count >= 2)
                {
                    wars = stats.Last().WarsCompleted - stats.First().WarsCompleted;
                    hours = stats.Last().HoursPlayed - stats.First().HoursPlayed;
                }

                leaderboard.Add(
                    new GuildMemberLeaderboardGetDTO
                    {
                        Id = m.GuildMemberId,
                        Name = m.MainUsername,
                        MinecraftUsername = m.MinecraftUsername,
                        Uuid = m.Uuid,
                        WarsCompleted = wars,
                        HoursPlayed = hours,
                        RaidsCompleted = raidCounts.TryGetValue(m.GuildMemberId, out var count)
                            ? count
                            : 0,
                        LastSynced = m.LastSynced,
                    }
                );
            }

            return leaderboard;
        }

        /* ============================
         * DELETE
         * ============================ */

        public async Task DeleteAsync(int id)
        {
            var member =
                await _context.GuildMembers.FindAsync(id) ?? throw new KeyNotFoundException();

            _context.GuildMembers.Remove(member);
            await _context.SaveChangesAsync();
        }

        /* ============================
         * HELPERS
         * ============================ */

        private async Task ValidateIdsAsync(GuildMemberPostDTO dto)
        {
            if (dto.Medals?.Any() == true)
            {
                var valid = await _context.Medals.Select(m => m.MedalId).ToListAsync();
                if (dto.Medals.Except(valid).Any())
                    throw new InvalidOperationException("Invalid MedalId");
            }

            if (dto.Games?.Any() == true)
            {
                var valid = await _context.Games.Select(g => g.GameId).ToListAsync();
                if (dto.Games.Except(valid).Any())
                    throw new InvalidOperationException("Invalid GameId");
            }
        }

        private async Task<GuildMemberAdminGetDTO> LoadAdminDtoAsync(int id) =>
            ToAdminDto(
                await _context
                    .GuildMembers.Include(m => m.Rank)
                    .Include(m => m.Games)
                        .ThenInclude(g => g.Game)
                    .Include(m => m.Medals)
                        .ThenInclude(m => m.Medal)
                    .FirstAsync(m => m.GuildMemberId == id)
            );

        private static GuildMemberAdminGetDTO ToAdminDto(GuildMember m) =>
            new()
            {
                Id = m.GuildMemberId,
                Name = m.MainUsername,
                DiscordTag = m.DiscordTag,
                Uuid = m.Uuid,
                JoinDate = m.JoinDate,
                Rank = new GenericGetDTO { Id = m.RankId, Name = m.Rank!.RankName },
                Games = m
                    .Games.Select(g => new GenericGetDTO { Id = g.GameId, Name = g.Game!.GameName })
                    .ToList(),
                Medals = m
                    .Medals.Select(mm => new GenericGetDTO
                    {
                        Id = mm.MedalId,
                        Name = mm.Medal!.MedalName,
                    })
                    .ToList(),
            };

        private static void SyncCollection<T>(
            ICollection<T> existing,
            IEnumerable<int>? targetIds,
            Func<T, int> getId,
            Func<int, T> factory
        )
            where T : class
        {
            targetIds ??= Enumerable.Empty<int>();

            foreach (var item in existing.Where(e => !targetIds.Contains(getId(e))).ToList())
                existing.Remove(item);

            var existingIds = existing.Select(getId).ToHashSet();

            foreach (var id in targetIds.Where(id => !existingIds.Contains(id)))
                existing.Add(factory(id));
        }
    }
}
