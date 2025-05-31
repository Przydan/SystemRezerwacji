namespace Domain.Entities;

public class Feature
{
    public Guid Id { get; set; }
    public string Name { get; set; } // Np. "Projektor", "Tablica interaktywna", "Klimatyzacja"
    public string? Description { get; set; } // Opcjonalny opis cechy

    // Właściwość nawigacyjna do tabeli łączącej ResourceFeature
    // Pokazuje, które zasoby posiadają tę cechę.
    public virtual ICollection<ResourceFeature> ResourceFeatures { get; set; } = new List<ResourceFeature>();
}