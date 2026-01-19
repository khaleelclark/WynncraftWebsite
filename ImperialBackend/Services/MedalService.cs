using ImperialBackend.DTOs;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

public class MedalService : IMedalService
{
    private readonly ImperialDbContext _context;

    public MedalService(ImperialDbContext context)
    {
        _context = context;
    }

    public async Task<List<GenericGetDTO>> GetAllAsync() =>
        await _context
            .Medals.AsNoTracking()
            .Select(m => new GenericGetDTO { Id = m.MedalId, Name = m.MedalName })
            .ToListAsync();

    public async Task<GenericGetDTO> CreateAsync(GenericPostDTO dto)
    {
        // Simple create; no uniqueness constraint enforced here.
        var medal = new Medal { MedalName = dto.Name };
        _context.Medals.Add(medal);
        await _context.SaveChangesAsync();

        return new GenericGetDTO { Id = medal.MedalId, Name = medal.MedalName };
    }

    public async Task<GenericGetDTO> UpdateAsync(int id, GenericPostDTO dto)
    {
        var medal = await _context.Medals.FindAsync(id) ?? throw new KeyNotFoundException();

        // Only name is editable.
        medal.MedalName = dto.Name;
        await _context.SaveChangesAsync();

        return new GenericGetDTO { Id = medal.MedalId, Name = medal.MedalName };
    }

    public async Task DeleteAsync(int id)
    {
        // Prevent deleting medals that are still assigned to members.
        var refs = await _context.GuildMemberMedals.AsNoTracking().CountAsync(m => m.MedalId == id);

        if (refs > 0)
            throw new InvalidOperationException(
                $"Medal {id} can't be deleted because {refs} guild member record(s) reference it."
            );

        var medal = await _context.Medals.FindAsync(id) ?? throw new KeyNotFoundException();

        _context.Medals.Remove(medal);
        await _context.SaveChangesAsync();
    }
}
