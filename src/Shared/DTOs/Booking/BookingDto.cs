namespace Shared.DTOs.Booking
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public string ResourceName { get; set; } = string.Empty; // Nazwa zasobu
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty; // Status jako string dla uproszczenia

        // --- DODANE/ZWERYFIKOWANE POLA ---
        public string? Notes { get; set; }
        public string? UserName { get; set; } // Nazwa użytkownika, który dokonał rezerwacji
        public Guid UserId { get; set; } // ID użytkownika
        public Guid ResourceId { get; set; } // ID rezerwowanego zasobu
    }
}