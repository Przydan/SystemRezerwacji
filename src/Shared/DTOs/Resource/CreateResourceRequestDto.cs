using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Shared.DTOs.Resource;

public class CreateResourceRequestDto
{
    [Required(ErrorMessage = "Nazwa zasobu jest wymagana.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Typ zasobu jest wymagany.")]
    public Guid ResourceTypeId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(1, 1000, ErrorMessage = "Pojemność musi być większa od 0.")]
    public int Capacity { get; set; }
    
    public IFormFile? Image { get; set; }
    public string? ImagePath { get; set; }

    // Opcjonalne dodatkowe pola, które możesz chcieć ustawiać przy tworzeniu.
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;
}