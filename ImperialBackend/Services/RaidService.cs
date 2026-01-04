using ImperialBackend.DTOs;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

public class RaidService : IRaidService
{
    private readonly ImperialDbContext _context;

    public RaidService(ImperialDbContext context)
    {
        _context = context;
    }

    public async Task<List<RaidGetDTO>> GetAllAsync() =>
        await _context
            .Raids.AsNoTracking()
            .Select(r => new RaidGetDTO
            {
                Id = r.RaidId,
                Name = r.RaidName,
                SeasonRating = r.SeasonRating,
            })
            .ToListAsync();

    public async Task<RaidGetDTO> CreateAsync(RaidPostDTO dto)
    {
        var raid = new Raid
        {
            RaidId = dto.Id,
            RaidName = dto.Name,
            SeasonRating = dto.SeasonRating,
        };

        _context.Raids.Add(raid);
        await _context.SaveChangesAsync();

        return new RaidGetDTO
        {
            Id = raid.RaidId,
            Name = raid.RaidName,
            SeasonRating = raid.SeasonRating,
        };
    }

    public async Task<RaidGetDTO> UpdateAsync(int id, RaidPostDTO dto)
    {
        var raid = await _context.Raids.FindAsync(id) ?? throw new KeyNotFoundException();

        raid.RaidName = dto.Name;
        raid.SeasonRating = dto.SeasonRating;

        await _context.SaveChangesAsync();

        return new RaidGetDTO
        {
            Id = raid.RaidId,
            Name = raid.RaidName,
            SeasonRating = raid.SeasonRating,
        };
    }

    public async Task DeleteAsync(int id)
    {
        var refs = await _context.RaidsCompleted.AsNoTracking().CountAsync(ri => ri.RaidId == id);

        if (refs > 0)
            throw new InvalidOperationException(
                $"Raid {id} can't be deleted because {refs} RaidInstances reference it."
            );

        var raid = await _context.Raids.FindAsync(id) ?? throw new KeyNotFoundException();

        _context.Raids.Remove(raid);
        await _context.SaveChangesAsync();
    }
}
