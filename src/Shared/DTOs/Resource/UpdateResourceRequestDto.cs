using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Shared.DTOs.Resource;

public class UpdateResourceRequestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Range(1, 1000, ErrorMessage = "Pojemność musi być większa od 0.")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Typ zasobu jest wymagany.")]
    public Guid ResourceTypeId { get; set; }

    public bool IsActive { get; set; }
    
    public IFormFile? Image { get; set; }
    public string? ImagePath { get; set; }
}