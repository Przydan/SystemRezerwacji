using Shared.DTOs.Booking;

namespace WebApp.Interfaces;

public interface IWebAppBookingService
{
    Task<List<BookingDto>> GetMyBookingsAsync(); 
    Task<BookingDto?> CreateBookingAsync(BookingRequestDto bookingRequest);
    Task<bool> CancelBookingAsync(Guid bookingId);
    Task<bool> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request);
    Task<BookingDto?> GetBookingByIdAsync(Guid bookingId);
    Task<List<BookingDto>> GetBookingsForResourceAsync(Guid resourceId);

    
    Task<bool> AdminCancelBookingAsync(Guid bookingId);
    Task<List<BookingDto>?> GetBookingsForUserAsync(Guid userId);

}