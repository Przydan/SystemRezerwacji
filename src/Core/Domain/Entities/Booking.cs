using Domain.Enums;

namespace Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } 
    public Guid ResourceId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }
    public required Resource Resource { get; set; }
    public required User User { get; set; }
    public string? Notes { get; set; }
    public Guid? RecurrenceGroupId { get; set; } // For recurring bookings
}