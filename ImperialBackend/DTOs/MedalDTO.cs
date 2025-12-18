using System.ComponentModel.DataAnnotations;

namespace ImperialBackend.Models
{
    public class MedalDTO
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string MedalName { get; set; } = default!;
}
}
