namespace SystemRezerwacji.Domain.Entities;

public class Resource
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public int? Capacity { get; set; }
    public bool IsActive { get; set; } = true;
    
    public Guid ResourceTypeId { get; set; }
    public ResourceType ResourceType { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<ResourceFeature> ResourceFeatures { get; set; } = new List<ResourceFeature>();
}