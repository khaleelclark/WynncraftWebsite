using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
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
            // Admin list includes raid + member details for editing.
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
            // Validate FK constraints up front for clearer errors.
            if (!await _context.Raids.AnyAsync(r => r.RaidId == dto.Raid))
                throw new InvalidOperationException("Invalid RaidId");

            // Enforce at least one guild member and validate all ids.
            if (dto.GuildMembers == null || dto.GuildMembers.Count == 0)
                throw new InvalidOperationException(
                    "GuildMemberIds must contain at least one member"
                );

            var validMemberIds = await _context
                .GuildMembers.Select(m => m.GuildMemberId)
                .ToListAsync();
            if (dto.GuildMembers.Except(validMemberIds).Any())
                throw new InvalidOperationException("Invalid GuildMemberId");

            var rc = await _context
                .RaidsCompleted.Include(r => r.RaidInstances)
                .FirstOrDefaultAsync(r => r.RaidCompletedId == id);

            if (rc == null)
                throw new KeyNotFoundException($"RaidCompleted {id} not found");

            rc.RaidId = dto.Raid;
            rc.CompletedDate = dto.CompletedDate;

            // Sync join table to match requested members.
            SyncCollection(
                rc.RaidInstances,
                dto.GuildMembers,
                ri => ri.GuildMemberId,
                guildMemberId => new RaidInstance { GuildMemberId = guildMemberId }
            );

            await _context.SaveChangesAsync();

            return await LoadGetDtoAsync(id);
        }

        public async Task<RaidCompletedGetDTO> CreateAsync(RaidCompletedPostDTO dto)
        {
            // Validate FK constraints up front for clearer errors.
            if (!await _context.Raids.AnyAsync(r => r.RaidId == dto.Raid))
                throw new InvalidOperationException("Invalid RaidId");

            // Enforce at least one guild member and validate all ids.
            if (dto.GuildMembers == null || dto.GuildMembers.Count == 0)
                throw new InvalidOperationException(
                    "GuildMemberIds must contain at least one member"
                );

            var validMemberIds = await _context
                .GuildMembers.Select(m => m.GuildMemberId)
                .ToListAsync();
            if (dto.GuildMembers.Except(validMemberIds).Any())
                throw new InvalidOperationException("Invalid GuildMemberId");

            var raidCompleted = new RaidCompleted
            {
                RaidId = dto.Raid,
                CompletedDate = dto.CompletedDate,
                // Create join rows for each unique member id.
                RaidInstances = dto
                    .GuildMembers.Distinct()
                    .Select(id => new RaidInstance { GuildMemberId = id })
                    .ToList(),
            };

            _context.RaidsCompleted.Add(raidCompleted);
            await _context.SaveChangesAsync();

            return await LoadGetDtoAsync(raidCompleted.RaidCompletedId);
        }

        public async Task<List<RaidCompletedPublicGetDTO>> GetAllPublicAsync(
            DateTimeOffset? startDate,
            DateTimeOffset? endDate
        )
        {
            // Public list is read-only but still includes names for display.
            var query = _context
                .RaidsCompleted.AsNoTracking()
                .Include(r => r.Raid)
                .Include(r => r.RaidInstances)
                    .ThenInclude(ri => ri.GuildMember)
                .AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (endDate < startDate)
                    throw new InvalidOperationException("endDate must be >= startDate");

                // Compare in UTC to align with persisted CompletedDate.
                var startUtc = startDate.Value.UtcDateTime;
                var endUtc = endDate.Value.UtcDateTime;

                // CompletedDate is stored in UTC.
                query = query.Where(r => r.CompletedDate >= startUtc && r.CompletedDate <= endUtc);
            }

            var raidsCompleted = await query.OrderByDescending(r => r.CompletedDate).ToListAsync();

            return raidsCompleted.Select(ToPublicDto).ToList();
        }

        public async Task DeleteAsync(int id)
        {
            // Delete join rows explicitly before the parent record.
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
            // Bot reports are idempotent-ish; we only create a new completion record.
            if (dto.MinecraftUsernames == null || dto.MinecraftUsernames.Count == 0)
                throw new InvalidOperationException(
                    "minecraftUsernames must contain at least one player."
                );

            var raid = await _context.Raids.FindAsync(dto.RaidId);
            if (raid == null)
                throw new InvalidOperationException($"Unknown RaidId {dto.RaidId}");

            var completedDateUtc = (dto.CompletedDate ?? DateTimeOffset.UtcNow).UtcDateTime;

            var raidCompleted = new RaidCompleted
            {
                RaidId = dto.RaidId,
                CompletedDate = completedDateUtc,
            };

            var notFoundUsers = new List<string>();

            // De-dupe incoming usernames to avoid duplicate RaidInstances.
            var distinctNames = dto
                .MinecraftUsernames.Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var members = await _context
                .GuildMembers.Where(m => m.MinecraftUsername != null)
                .Select(m => new { m.GuildMemberId, m.MinecraftUsername })
                .ToListAsync();

            // Match usernames case-insensitively to stored MinecraftUsername.
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

            return new
            {
                // Return details for bot logging/confirmation.
                raidId = raid.RaidId,
                raidName = raid.RaidName,
                raidInstanceId = raidCompleted.RaidCompletedId,
                completedDateUtc,
                createdCount = raidCompleted.RaidInstances.Count,
                // Returned for bot feedback/logging.
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

        private static RaidCompletedPublicGetDTO ToPublicDto(RaidCompleted rc) =>
            new()
            {
                Id = rc.RaidCompletedId,
                CompletedDate = rc.CompletedDate,
                RaidName = rc.Raid?.RaidName ?? "(Unknown Raid)",
                GuildMembers = rc
                    .RaidInstances.Where(ri => ri.GuildMember?.MainUsername != null)
                    .Select(ri => ri.GuildMember!.MainUsername!)
                    .Distinct()
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
