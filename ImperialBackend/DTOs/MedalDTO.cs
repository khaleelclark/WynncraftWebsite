using System.ComponentModel.DataAnnotations;

namespace ImperialBackend.Models
{
    public class MedalDTO
{
    [Required]
    [MinLength(1)]
    public string MedalName { get; set; } = default!;
}
}
