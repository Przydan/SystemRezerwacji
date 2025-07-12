namespace Domain.Entities;

public class Resource
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid ResourceTypeId { get; set; }
    public required ResourceType ResourceType { get; set; }
    public ICollection<ResourceFeature> Features { get; set; } = new List<ResourceFeature>();
    public IEnumerable<Booking>? Bookings { get; set; }
    public IEnumerable<ResourceFeature>? ResourceFeatures { get; set; }
}