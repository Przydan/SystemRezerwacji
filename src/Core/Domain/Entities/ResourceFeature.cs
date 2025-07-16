namespace Domain.Entities;

public class  ResourceFeature
{
    public Guid ResourceId { get; set; }
    public virtual required Resource Resource { get; set; }
    public Guid FeatureId { get; set; }
    public virtual required Feature Feature { get; set; }
}