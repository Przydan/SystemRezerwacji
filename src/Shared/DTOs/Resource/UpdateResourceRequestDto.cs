using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Resource;

public class UpdateResourceRequestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public int Capacity { get; set; }
    [Required(ErrorMessage = "Typ zasobu jest wymagany.")]
    public Guid ResourceTypeId { get; set; }
}