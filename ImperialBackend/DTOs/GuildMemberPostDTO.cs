using System.ComponentModel.DataAnnotations;

namespace ImperialBackend.Models
{
    public class GuildMemberPostDTO
    {
        public string? DiscordTag { get; set; }
        public string? MainUsername { get; set; }

        [Required]
        public DateTime JoinDate { get; set; }

        [Required]
        public Guid Uuid { get; set; }
    }
}
