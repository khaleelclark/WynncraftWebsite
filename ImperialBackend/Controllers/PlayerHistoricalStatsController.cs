using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    public class PlayerHistoricalStatsController : ODataController
    {
        private readonly ImperialDbContext _context;
        public PlayerHistoricalStatsController(ImperialDbContext context) => _context = context;

        [EnableQuery]
        [HttpGet]
        public IQueryable<PlayerHistoricalStat> Get() => _context.PlayerHistoricalStats.Include(p => p.GuildMember);


        [HttpPut]
        public IActionResult Put([FromBody] PlayerHistoricalStat stat)
        {
            _context.Entry(stat).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(stat);
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] PlayerHistoricalStat stat)
        {
            _context.PlayerHistoricalStats.Remove(stat);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
