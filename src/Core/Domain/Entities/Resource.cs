namespace Domain.Entities;

public class Resource
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; } = "";
    public int? Capacity { get; set; }
    public Guid ResourceTypeId { get; set; }
    public required ResourceType ResourceType { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<ResourceFeature> ResourceFeatures { get; set; } = new List<ResourceFeature>();
}