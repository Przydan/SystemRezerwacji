namespace Shared.DTOs.Resource
{
    public class ResourceDto
    {
        public ResourceDto()
        {
        }
        
        public ResourceDto(ResourceDto resource)
        {
            Id = resource.Id;
            Name = resource.Name;
            Description = resource.Description;
            Location = resource.Location;
            Capacity = resource.Capacity;
            IsActive = resource.IsActive;
            ImagePath = resource.ImagePath;
            ResourceTypeId = resource.ResourceTypeId;
            ResourceTypeName = resource.ResourceTypeName;
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = "";
        public string Location { get; set; } = "";
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public string? ImagePath { get; set; }
        public Guid ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; } = "";
    }
}