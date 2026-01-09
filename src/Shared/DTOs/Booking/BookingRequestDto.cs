using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Booking
{
    public class BookingRequestDto
    {
        [Required] public Guid ResourceId { get; set; }

        [Required] public DateTime? StartTime { get; set; }

        [Required] public DateTime? EndTime { get; set; }

        public string? Notes { get; set; }

        // Recurrence Options
        public bool IsRecurring { get; set; }
        public string? Frequency { get; set; } // "Daily", "Weekly"
        public int? Occurrences { get; set; }
    }
}