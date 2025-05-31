namespace Domain.Entities;

public class ResourceType
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? IconCssClass { get; set; } // Opcjonalne
    
    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}