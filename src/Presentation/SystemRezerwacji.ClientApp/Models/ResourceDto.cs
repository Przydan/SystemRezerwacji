namespace SystemRezerwacji.ClientApp.Models
{
    // SystemRezerwacji.ClientApp/Models/ResourceDto.cs
    using System;

    public class ResourceDto
    {
        public Guid Id { get; set; } // Zakładając, że ID jest GUIDem
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        // Możesz dodać więcej pól, jeśli API je zwraca i chcesz je od razu wyświetlić,
        // np. Location, Capacity, IsActive, ResourceTypeName
        // Na razie, zgodnie z wymaganiem MVP, wystarczą Name i Description.
    }
}
