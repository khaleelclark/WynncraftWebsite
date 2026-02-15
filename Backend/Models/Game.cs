using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }
        public string? GameName { get; set; }
        public ICollection<GuildMemberGame> GuildMemberGames { get; set; } =
            new List<GuildMemberGame>();
    }
}
