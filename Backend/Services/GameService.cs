using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

public class GameService : IGameService
{
    private readonly WynncraftDbContext _context;

    public GameService(WynncraftDbContext context)
    {
        _context = context;
    }

    public async Task<List<GenericGetDTO>> GetAllAsync() =>
        await _context
            .Games.AsNoTracking()
            .Select(g => new GenericGetDTO { Id = g.GameId, Name = g.GameName })
            .ToListAsync();

    public async Task<GenericGetDTO> CreateAsync(GenericPostDTO dto)
    {
        // Enforce unique game names.
        if (await _context.Games.AnyAsync(g => g.GameName == dto.Name))
            throw new InvalidOperationException("Game already exists");

        var game = new Game { GameName = dto.Name };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return new GenericGetDTO { Id = game.GameId, Name = game.GameName };
    }

    public async Task<GenericGetDTO> UpdateAsync(int id, GenericPostDTO dto)
    {
        var game = await _context.Games.FindAsync(id) ?? throw new KeyNotFoundException();

        // Only name is editable.
        game.GameName = dto.Name;
        await _context.SaveChangesAsync();

        return new GenericGetDTO { Id = game.GameId, Name = game.GameName };
    }

    public async Task DeleteAsync(int id)
    {
        // Prevent deleting games still referenced by members.
        var refs = await _context.GuildMemberGames.AsNoTracking().CountAsync(g => g.GameId == id);

        if (refs > 0)
            throw new InvalidOperationException(
                $"Game {id} can't be deleted because {refs} guild member record(s) reference it."
            );

        var game = await _context.Games.FindAsync(id) ?? throw new KeyNotFoundException();

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
    }
}
