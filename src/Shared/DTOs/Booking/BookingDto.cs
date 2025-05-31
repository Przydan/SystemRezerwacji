namespace Shared.DTOs.Booking
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? UserName { get; set; } // Dodane pole
        public string? Notes { get; set; }    // Dodane pole
        public Guid UserId { get; set; } // Może być przydatne
        public Guid ResourceId { get; set; }
    }
}