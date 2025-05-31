using Domain.Enums;

namespace Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime BookedAt { get; set; } = DateTime.UtcNow; // Domyślnie czas utworzenia
    public DateTime? LastModifiedAt { get; set; } // Opcjonalne
    public string? Notes { get; set; }

    public BookingStatus Status { get; set; } // Enum, który zdefiniujemy

    // Klucz obcy i nawigacja do Resource
    public Guid ResourceId { get; set; }
    public Resource Resource { get; set; }

    // Klucz obcy i nawigacja do User
    public Guid UserId { get; set; } // Na razie Guid, później połączymy z IdentityUser
    public User User { get; set; } // Właściwość nawigacyjna do encji User, którą dodamy
}