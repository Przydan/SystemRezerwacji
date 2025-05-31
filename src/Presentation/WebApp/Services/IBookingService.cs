using Shared.DTOs.Booking;

namespace WebApp.Services
{
    public interface IBookingService
    {
        Task CreateBookingAsync(BookingRequestDto dto);
        Task<List<BookingDto>> GetMyBookingsAsync();
        Task CancelBookingAsync(Guid bookingId);
    }
}