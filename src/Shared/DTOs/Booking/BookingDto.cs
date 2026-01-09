namespace Shared.DTOs.Booking
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? UserName { get; set; }
        public Guid UserId { get; set; }
        public Guid ResourceId { get; set; }
        public Guid? RecurrenceGroupId { get; set; }
    }
}