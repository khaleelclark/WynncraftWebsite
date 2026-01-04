using ImperialBackend.DTOs;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Services
{
    public class RaidsCompletedService : IRaidsCompletedService
    {
        private readonly ImperialDbContext _context;

        public RaidsCompletedService(ImperialDbContext context)
        {
            _context = context;
        }

        public async Task<List<RaidCompletedGetDTO>> GetAllAsync()
        {
            var raidsCompleted = await _context
                .RaidsCompleted.AsNoTracking()
                .Include(r => r.Raid)
                .Include(r => r.RaidInstances)
                    .ThenInclude(ri => ri.GuildMember)
                .OrderByDescending(r => r.CompletedDate)
                .ToListAsync();

            return raidsCompleted.Select(ToGetDto).ToList();
        }

        public async Task<RaidCompletedGetDTO> GetByIdAsync(int id)
        {
            var rc = await _context
                .RaidsCompleted.AsNoTracking()
                .Include(r => r.Raid)
                .Include(r => r.RaidInstances)
                    .ThenInclude(ri => ri.GuildMember)
                .FirstOrDefaultAsync(r => r.RaidCompletedId == id);

            if (rc == null)
                throw new KeyNotFoundException($"RaidCompleted {id} not found");

            return ToGetDto(rc);
        }

        public async Task<RaidCompletedGetDTO> UpdateAsync(int id, RaidCompletedPostDTO dto)
        {
            if (!await _context.Raids.AnyAsync(r => r.RaidId == dto.Raid))
                throw new InvalidOperationException("Invalid RaidId");

            if (dto.GuildMembers == null || dto.GuildMembers.Count == 0)
                throw new InvalidOperationException(
                    "GuildMemberIds must contain at least one member"
                );

            var validMemberIds = await _context
                .GuildMembers.Select(m => m.GuildMemberId)
                .ToListAsync();

            if (dto.GuildMembers.Except(validMemberIds).Any())
                throw new InvalidOperationException("Invalid GuildMemberId");

            await using var tx = await _context.Database.BeginTransactionAsync();

            var rc = await _context
                .RaidsCompleted.Include(r => r.RaidInstances)
                .FirstOrDefaultAsync(r => r.RaidCompletedId == id);

            if (rc == null)
                throw new KeyNotFoundException($"RaidCompleted {id} not found");

            rc.RaidId = dto.Raid;
            rc.CompletedDate = dto.CompletedDate;

            SyncCollection(
                rc.RaidInstances,
                dto.GuildMembers,
                ri => ri.GuildMemberId,
                guildMemberId => new RaidInstance { GuildMemberId = guildMemberId }
            );

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return await LoadGetDtoAsync(id);
        }

        public async Task<RaidCompletedGetDTO> CreateAsync(RaidCompletedPostDTO dto)
        {
            if (!await _context.Raids.AnyAsync(r => r.RaidId == dto.Raid))
                throw new InvalidOperationException("Invalid RaidId");

            if (dto.GuildMembers == null || dto.GuildMembers.Count == 0)
                throw new InvalidOperationException(
                    "GuildMemberIds must contain at least one member"
                );

            var validMemberIds = await _context
                .GuildMembers.Select(m => m.GuildMemberId)
                .ToListAsync();

            if (dto.GuildMembers.Except(validMemberIds).Any())
                throw new InvalidOperationException("Invalid GuildMemberId");

            await using var tx = await _context.Database.BeginTransactionAsync();

            var raidCompleted = new RaidCompleted
            {
                RaidId = dto.Raid,
                CompletedDate = dto.CompletedDate,
                RaidInstances = dto
                    .GuildMembers.Distinct()
                    .Select(id => new RaidInstance { GuildMemberId = id })
                    .ToList(),
            };

            _context.RaidsCompleted.Add(raidCompleted);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return await LoadGetDtoAsync(raidCompleted.RaidCompletedId);
        }

        public async Task DeleteAsync(int id)
        {
            var rc = await _context
                .RaidsCompleted.Include(r => r.RaidInstances)
                .FirstOrDefaultAsync(r => r.RaidCompletedId == id);

            if (rc == null)
                throw new KeyNotFoundException($"RaidCompleted {id} not found");

            _context.RaidInstances.RemoveRange(rc.RaidInstances);
            _context.RaidsCompleted.Remove(rc);

            await _context.SaveChangesAsync();
        }

        public async Task<object> SyncFromBotAsync(RaidBotReportDTO dto)
        {
            if (dto.MinecraftUsernames == null || dto.MinecraftUsernames.Count == 0)
                throw new InvalidOperationException(
                    "minecraftUsernames must contain at least one player."
                );

            var raid = await _context.Raids.FindAsync(dto.RaidId);
            if (raid == null)
                throw new InvalidOperationException($"Unknown RaidId {dto.RaidId}");

            var completedDateUtc = (dto.CompletedDate ?? DateTimeOffset.UtcNow).UtcDateTime;

            await using var tx = await _context.Database.BeginTransactionAsync();

            var raidCompleted = new RaidCompleted
            {
                RaidId = dto.RaidId,
                CompletedDate = completedDateUtc,
            };

            var notFoundUsers = new List<string>();

            var distinctNames = dto
                .MinecraftUsernames.Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var members = await _context
                .GuildMembers.Where(m => m.MinecraftUsername != null)
                .Select(m => new { m.GuildMemberId, m.MinecraftUsername })
                .ToListAsync();

            foreach (var username in distinctNames)
            {
                var member = members.FirstOrDefault(m =>
                    string.Equals(m.MinecraftUsername, username, StringComparison.OrdinalIgnoreCase)
                );

                if (member == null)
                {
                    notFoundUsers.Add(username);
                    continue;
                }

                raidCompleted.RaidInstances.Add(
                    new RaidInstance { GuildMemberId = member.GuildMemberId }
                );
            }

            _context.RaidsCompleted.Add(raidCompleted);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return new
            {
                raidId = raid.RaidId,
                raidName = raid.RaidName,
                raidInstanceId = raidCompleted.RaidCompletedId,
                completedDateUtc,
                createdCount = raidCompleted.RaidInstances.Count,
                notFoundUsers,
            };
        }

        private async Task<RaidCompletedGetDTO> LoadGetDtoAsync(int id)
        {
            var rc = await _context
                .RaidsCompleted.AsNoTracking()
                .Include(r => r.Raid)
                .Include(r => r.RaidInstances)
                    .ThenInclude(ri => ri.GuildMember)
                .FirstAsync(r => r.RaidCompletedId == id);

            return ToGetDto(rc);
        }

        private static RaidCompletedGetDTO ToGetDto(RaidCompleted rc) =>
            new()
            {
                Id = rc.RaidCompletedId,
                CompletedDate = rc.CompletedDate,
                Raid = new GenericGetDTO
                {
                    Id = rc.RaidId,
                    Name = rc.Raid?.RaidName ?? "(Unknown Raid)",
                },
                GuildMembers = rc
                    .RaidInstances.Where(ri => ri.GuildMember != null)
                    .Select(ri => new GenericGetDTO
                    {
                        Id = ri.GuildMemberId,
                        Name = ri.GuildMember!.MainUsername,
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
