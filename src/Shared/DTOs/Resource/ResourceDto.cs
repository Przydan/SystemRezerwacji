namespace Shared.DTOs.Resource
{
    public class ResourceDto
    {
        // Konstruktor domyślny
        public ResourceDto()
        {
            // Domyślne wartości są już ustawione przy deklaracji właściwości
        }
        
        // Konstruktor kopiujący
        public ResourceDto(ResourceDto resource)
        {
            if (resource == null)
                return;
                
            Id = resource.Id;
            Name = resource.Name;
            Description = resource.Description;
            Location = resource.Location;
            Capacity = resource.Capacity;
            IsActive = resource.IsActive;
            ResourceTypeId = resource.ResourceTypeId;
            ResourceTypeName = resource.ResourceTypeName;
        }
        
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Location { get; set; } = "";
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public Guid ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; } = "";
    }
}