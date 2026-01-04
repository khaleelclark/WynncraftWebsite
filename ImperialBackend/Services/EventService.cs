using ImperialBackend.DTOs;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

public class EventService : IEventService
{
    private readonly ImperialDbContext _context;

    public EventService(ImperialDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventGetDTO>> GetAllAsync() =>
        await _context
            .Events.AsNoTracking()
            .Select(e => new EventGetDTO
            {
                Id = e.EventId,
                Name = e.EventName,
                EventStart = e.EventStart,
                EventEnd = e.EventEnd,
            })
            .ToListAsync();

    public async Task<EventGetDTO> GetByIdAsync(int id)
    {
        var ev = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.EventId == id);

        if (ev == null)
            throw new KeyNotFoundException();

        return new EventGetDTO
        {
            Id = ev.EventId,
            Name = ev.EventName,
            EventStart = ev.EventStart,
            EventEnd = ev.EventEnd,
        };
    }

    public async Task<EventGetDTO> CreateAsync(EventPostDTO dto)
    {
        var ev = new Event
        {
            EventName = dto.Name,
            EventStart = dto.EventStart,
            EventEnd = dto.EventEnd,
        };

        _context.Events.Add(ev);
        await _context.SaveChangesAsync();

        return new EventGetDTO
        {
            Id = ev.EventId,
            Name = ev.EventName,
            EventStart = ev.EventStart,
            EventEnd = ev.EventEnd,
        };
    }

    public async Task<EventGetDTO> UpdateAsync(int id, EventPostDTO dto)
    {
        var ev = await _context.Events.FindAsync(id) ?? throw new KeyNotFoundException();

        ev.EventName = dto.Name;
        ev.EventStart = dto.EventStart;
        ev.EventEnd = dto.EventEnd;

        await _context.SaveChangesAsync();

        return new EventGetDTO
        {
            Id = ev.EventId,
            Name = ev.EventName,
            EventStart = ev.EventStart,
            EventEnd = ev.EventEnd,
        };
    }

    public async Task DeleteAsync(int id)
    {
        var ev = await _context.Events.FindAsync(id) ?? throw new KeyNotFoundException();

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
    }
}
