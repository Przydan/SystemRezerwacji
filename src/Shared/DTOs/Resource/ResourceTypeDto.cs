namespace Shared.DTOs.Resource;

public class ResourceTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconCssClass { get; set; }
}