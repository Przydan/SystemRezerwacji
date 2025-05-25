namespace SystemRezerwacji.ClientApp.Models
{
    public class ResourceTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? IconCssClass { get; set; }
    }
}