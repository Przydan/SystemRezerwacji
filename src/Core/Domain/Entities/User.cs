using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // Nawigacja do rezerwacji
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

}