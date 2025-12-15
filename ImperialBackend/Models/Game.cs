using System.ComponentModel.DataAnnotations;
namespace ImperialBackend.Models
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }
        public string? GameName { get; set; }
        public ICollection<GuildMemberGame> GuildMemberGames { get; set; } = new List<GuildMemberGame>();
    }
}
