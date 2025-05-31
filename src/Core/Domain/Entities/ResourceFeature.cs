namespace Domain.Entities;

public class ResourceFeature
{
    // Klucze złożone (Composite Key) wskazujące na Resource i Feature
    public Guid ResourceId { get; set; }
    public virtual Resource Resource { get; set; } // Właściwość nawigacyjna do Resource

    public Guid FeatureId { get; set; }
    public virtual Feature Feature { get; set; } // Właściwość nawigacyjna do Feature

    // Możesz tu dodać dodatkowe właściwości specyficzne dla połączenia,
    // np. wartość cechy (jeśli cecha to np. "Liczba portów USB", a wartość to "4"),
    // ale na razie zostawmy to proste.
    // public string? Value { get; set; }
}