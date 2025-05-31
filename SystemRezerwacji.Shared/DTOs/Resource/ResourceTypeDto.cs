namespace SystemRezerwacji.Application.DTOs.ResourceType;

public class ResourceTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? IconCssClass { get; set; }
}