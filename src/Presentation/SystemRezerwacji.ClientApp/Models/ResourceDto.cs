using System.ComponentModel.DataAnnotations;

public class ResourceDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa jest wymagana")]
    public string Name { get; set; }

    public string Description { get; set; }
    public string Location { get; set; }

    [Range(1, 1000, ErrorMessage = "Pojemność musi być od 1 do 1000")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Typ zasobu jest wymagany")]
    public int ResourceTypeId { get; set; }

    // opcjonalnie do wyświetlania w tabeli
    public string ResourceTypeName { get; set; }

    public bool IsActive { get; set; }

    public ResourceDto() { }

    // konstruktor do kopiowania przy edycji
    public ResourceDto(ResourceDto other)
    {
        Id = other.Id;
        Name = other.Name;
        Description = other.Description;
        Location = other.Location;
        Capacity = other.Capacity;
        ResourceTypeId = other.ResourceTypeId;
        ResourceTypeName = other.ResourceTypeName;
        IsActive = other.IsActive;
    }
}