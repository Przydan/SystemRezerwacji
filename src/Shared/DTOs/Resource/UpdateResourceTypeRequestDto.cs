using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Resource;

public class UpdateResourceTypeRequestDto
{
    [Required(ErrorMessage = "Nazwa jest wymagana.")]
    [StringLength(100, ErrorMessage = "Nazwa nie może być dłuższa niż 100 znaków.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [StringLength(50, ErrorMessage = "Klasa CSS ikony nie może być dłuższa niż 50 znaków.")]
    public string? IconCssClass { get; set; }
}