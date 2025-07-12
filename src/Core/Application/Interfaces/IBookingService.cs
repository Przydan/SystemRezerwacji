using Shared.DTOs.Booking;

namespace Application.Interfaces.Booking;

public interface IBookingService
{
    Task<List<BookingDto>> GetUserBookingsAsync(Guid userId);
    Task<BookingDto?> CreateBookingAsync(BookingRequestDto bookingRequest, Guid userId);
    Task<bool> CancelBookingAsync(Guid bookingId, Guid userId);
    Task<bool> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request, Guid userId);
    Task<BookingDto?> GetBookingByIdAsync(Guid bookingId, Guid userId);
}