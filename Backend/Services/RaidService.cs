using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

public class RaidService : IRaidService
{
    private readonly WynncraftDbContext _context;

    public RaidService(WynncraftDbContext context)
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
        // Prevent duplicate ids in the source data.
        var exists = await _context.Raids.AnyAsync(r => r.RaidId == dto.Id);
        if (exists)
            throw new InvalidOperationException($"Raid with id {dto.Id} already exists.");

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
        // Block deletion if any completion records reference the raid.
        var refs = await _context.RaidsCompleted.AsNoTracking().CountAsync(ri => ri.RaidId == id);

        if (refs > 0)
            throw new InvalidOperationException(
                $"Raid {id} can't be deleted because {refs} Raid Instances reference it."
            );

        var raid = await _context.Raids.FindAsync(id) ?? throw new KeyNotFoundException();

        _context.Raids.Remove(raid);
        await _context.SaveChangesAsync();
    }
}
