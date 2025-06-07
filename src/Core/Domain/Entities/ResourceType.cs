namespace Domain.Entities;

public class ResourceType
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? IconCssClass { get; set; }

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}