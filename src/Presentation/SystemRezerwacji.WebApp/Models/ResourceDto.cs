using System;

namespace SystemRezerwacji.WebApp.Models
{
    public class ResourceDto
    {
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