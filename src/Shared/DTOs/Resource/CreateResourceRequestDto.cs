using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Resource;

public class CreateResourceRequestDto
{
    [Required(ErrorMessage = "Nazwa zasobu jest wymagana.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Typ zasobu jest wymagany.")]
    public Guid ResourceTypeId { get; set; }

    // Opcjonalne dodatkowe pola, które możesz chcieć ustawiać przy tworzeniu.
    public string? Location { get; set; }
    public int? Capacity { get; set; }
    public bool IsActive { get; set; } = true;
}