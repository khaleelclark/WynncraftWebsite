using System.ComponentModel.DataAnnotations;

namespace ImperialBackend.Models
{
    public class GameDTO
    {        
        [Required]
        [StringLength(100, MinimumLength = 1)] 
        public string? GameName { get; set; }
    }
}
