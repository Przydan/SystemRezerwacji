namespace Shared.DTOs.Resource;

public class UpdateResourceRequestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}