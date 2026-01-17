using Shared.DTOs.Booking;

namespace Web.ViewModels
{
    public class HomeViewModel
    {
        // Admin Stats
        public int TotalUsers { get; set; }
        public int TotalResources { get; set; }
        public int ActiveBookingsToday { get; set; }

        // User Data
        public List<BookingDto> UpcomingBookings { get; set; } = new List<BookingDto>();
    }
}
