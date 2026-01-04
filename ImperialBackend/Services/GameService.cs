using ImperialBackend.DTOs;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

public class GameService : IGameService
{
    private readonly ImperialDbContext _context;

    public GameService(ImperialDbContext context)
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

        game.GameName = dto.Name;
        await _context.SaveChangesAsync();

        return new GenericGetDTO { Id = game.GameId, Name = game.GameName };
    }

    public async Task DeleteAsync(int id)
    {
        if (await _context.GuildMemberGames.AnyAsync(g => g.GameId == id))
            throw new InvalidOperationException(
                "Cannot delete game with guild members. Remove guild members first"
            );

        var game = await _context.Games.FindAsync(id) ?? throw new KeyNotFoundException();

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
    }
}
