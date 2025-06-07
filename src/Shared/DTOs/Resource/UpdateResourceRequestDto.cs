namespace Shared.DTOs.Resource;

public class UpdateResourceRequestDto
{
    public Guid Id { get; set; } // Musi być Guid
    public string Name { get; set; }
}