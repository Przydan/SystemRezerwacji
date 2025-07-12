using Shared.DTOs.Booking;

namespace WebApp.Services;

public interface IBookingService
{
    Task<List<BookingDto>?> GetMyBookingsAsync();
    Task<BookingDto?> GetBookingByIdAsync(Guid bookingId);
    Task<BookingDto?> CreateBookingAsync(BookingRequestDto request);
    Task<bool> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request);
    Task<bool> CancelBookingAsync(Guid bookingId);
}