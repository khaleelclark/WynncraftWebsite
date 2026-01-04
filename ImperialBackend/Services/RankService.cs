using ImperialBackend.DTOs;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

public class RankService : IRankService
{
    private readonly ImperialDbContext _context;

    public RankService(ImperialDbContext context)
    {
        _context = context;
    }

    public async Task<List<GenericGetDTO>> GetAllAsync() =>
        await _context
            .Ranks.AsNoTracking()
            .Select(r => new GenericGetDTO { Id = r.RankId, Name = r.RankName })
            .ToListAsync();

    public async Task<GenericGetDTO> CreateAsync(GenericPostDTO dto)
    {
        var rank = new Rank { RankName = dto.Name };
        _context.Ranks.Add(rank);
        await _context.SaveChangesAsync();

        return new GenericGetDTO { Id = rank.RankId, Name = rank.RankName };
    }

    public async Task<GenericGetDTO> UpdateAsync(int id, GenericPostDTO dto)
    {
        var rank = await _context.Ranks.FindAsync(id) ?? throw new KeyNotFoundException();

        rank.RankName = dto.Name;
        await _context.SaveChangesAsync();

        return new GenericGetDTO { Id = rank.RankId, Name = rank.RankName };
    }

    public async Task DeleteAsync(int id)
    {
        var rank =
            await _context
                .Ranks.Include(r => r.GuildMembers)
                .FirstOrDefaultAsync(r => r.RankId == id)
            ?? throw new KeyNotFoundException();

        if (rank.GuildMembers.Any())
            throw new InvalidOperationException(
                "Can't delete this rank because it still has guild members. Reassign them first."
            );

        _context.Ranks.Remove(rank);
        await _context.SaveChangesAsync();
    }
}
