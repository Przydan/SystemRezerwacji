namespace SystemRezerwacji.Domain.Entities;

public class User
{
    public Guid Id { get; set; } // Później może być dziedziczone z IdentityUser<Guid>
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; } // Ważne dla identyfikacji

    // Nawigacja do rezerwacji
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

}