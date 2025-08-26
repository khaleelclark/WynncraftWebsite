using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ImperialBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ImperialBackend.Controllers
{
    public class GuildMembersController : ODataController
    {
        private readonly ImperialDbContext _context;
        private readonly Services.GuildMemberSyncService _syncService;
        public GuildMembersController(ImperialDbContext context, Services.GuildMemberSyncService syncService)
        {
            _context = context;
            _syncService = syncService;
        }

        // Leaderboard GET: Only displays people with wynncraft stats between start & end date
        [HttpGet]
        [Route("odata/GuildMembers/Leaderboard")]
        public IActionResult GetLeaderboard([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var statsRaw = _context.PlayerHistoricalStats
                .Include(phs => phs.GuildMember)
                .Where(phs => phs.SyncDate >= startDate && phs.SyncDate <= endDate)
                .ToList();

            var stats = statsRaw
                .GroupBy(phs => phs.GuildMember)
                .Select(g =>
                {
                    var ordered = g.OrderBy(phs => phs.SyncDate).ToList();
                    var first = ordered.FirstOrDefault();
                    var last = ordered.LastOrDefault();
                    return new
                    {
                        g.Key.PlayerSkin,
                        g.Key.MinecraftUsername,
                        WeekliesCompleted = (last != null && first != null) ? last.WeekliesCompleted - first.WeekliesCompleted : 0,
                        WarsCompleted = (last != null && first != null) ? last.WarsCompleted - first.WarsCompleted : 0,
                        HoursPlayed = (last != null && first != null) ? last.HoursPlayed - first.HoursPlayed : 0,
                        RaidsCompleted = _context.RaidsCompleted.Count(rc => rc.GuildMember.GuildMemberId == g.Key.GuildMemberId)
                    };
                })
                .ToList();
            return Ok(stats);
        }

        // Custom GET: All guild members with selected fields
        [HttpGet]
        [Route("odata/GuildMembers/CustomAll")]
        public IActionResult GetCustomAll()
        {
            var members = _context.GuildMembers
                .Include(g => g.Rank)
                .Include(g => g.Games).ThenInclude(gmg => gmg.Game)
                .Include(g => g.Medals).ThenInclude(gmm => gmm.Medal)
                .Select(g => new
                {
                    g.DiscordTag,
                    g.MainUsername,
                    g.MinecraftUsername,
                    RankName = g.Rank != null ? g.Rank.RankName : null,
                    g.PlayerSkin,
                    g.JoinDate,
                    g.Uuid,
                    g.WynncraftRank,
                    Games = g.Games.Select(x => x.Game.GameName).ToList(),
                    Medals = g.Medals.Select(x => x.Medal.MedalName).ToList()
                })
                .ToList();
            return Ok(members);
        }

        // Custom GET: Profile page for a single guild member
        [HttpGet]
        [Route("odata/GuildMembers/Profile/{guildMemberId}")]
        public IActionResult GetProfile(int guildMemberId)
        {
            var member = _context.GuildMembers
                .Include(g => g.Rank)
                .Include(g => g.Games).ThenInclude(gmg => gmg.Game)
                .Include(g => g.Medals).ThenInclude(gmm => gmm.Medal)
                .FirstOrDefault(g => g.GuildMemberId == guildMemberId);
            if (member == null) return NotFound();

            var raidsCompletedCount = _context.RaidsCompleted.Count(rc => rc.GuildMember != null && rc.GuildMember.GuildMemberId == guildMemberId);

            var profile = new
            {
                member.MainUsername,
                member.MinecraftUsername,
                RankName = member.Rank != null ? member.Rank.RankName : null,
                member.PlayerSkin,
                member.JoinDate,
                member.WynncraftRank,
                member.HoursPlayed,
                member.WarsCompleted,
                member.WeekliesCompleted,
                Games = member.Games.Select(x => x.Game.GameName).ToList(),
                Medals = member.Medals.Select(x => x.Medal.MedalName).ToList(),
                RaidsCompleted = raidsCompletedCount
            };
            return Ok(profile);
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<object> Get()
        {
            return _context.GuildMembers
                .Include(g => g.Rank)
                .Include(g => g.Games).ThenInclude(gmg => gmg.Game)
                .Include(g => g.Medals).ThenInclude(gmm => gmm.Medal)
                .Select(g => new
                {
                    guildMemberId = g.GuildMemberId,
                    discord_tag = g.DiscordTag,
                    main_username = g.MainUsername,
                    minecraft_username = g.MinecraftUsername,
                    rank_name = g.Rank != null ? g.Rank.RankName : null,
                    player_skin = g.PlayerSkin,
                    join_date = g.JoinDate,
                    uuid = g.Uuid,
                    wynncraft_rank = g.WynncraftRank,
                    games = g.Games.Select(x => x.Game.GameName).ToList(),
                    medals = g.Medals.Select(x => x.Medal.MedalName).ToList()
                });
        }

        [HttpPost]
        public IActionResult Post([FromBody] GuildMember member)
        {
            // Defensive: ensure navigation properties are not null
            // if (member.Games == null)
            //     member.Games = new List<GuildMemberGame>();
            //  if (member.Medals == null)
            //     member.Medals = new List<GuildMemberMedal>();
            // Rank is optional, but if present, ensure it is attached by ID
            //   if (member.Rank == null && member.RankId != 0)
            //      member.Rank = _context.Ranks.Find(member.RankId);

            _context.GuildMembers.Add(member);
            _context.SaveChanges();
            return Created(member);
        }

        [HttpPut]
        [Route("odata/GuildMembers")]
        public IActionResult Put([FromBody] GuildMember member)
        {
            _context.Entry(member).State = EntityState.Modified;
            _context.SaveChanges();
            return Updated(member);
        }

        [HttpDelete]
        [Route("odata/GuildMembers")]
        public IActionResult Delete([FromBody] GuildMember member)
        {
            _context.GuildMembers.Remove(member);
            _context.SaveChanges();
            return NoContent();
        }
    }
}