using Shared.DTOs.Booking;

namespace Application.Interfaces.Booking;

public interface IBookingService
{
    Task<List<BookingDto>> GetUserBookingsAsync(Guid userId);
    Task<BookingDto?> CreateBookingAsync(BookingRequestDto bookingRequest, Guid userId);
    Task<bool> CancelBookingAsync(Guid bookingId, Guid userId);
    Task<bool> AdminCancelBookingAsync(Guid bookingId);
    Task<bool> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request, Guid userId);
    Task<bool> AdminUpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request);
    Task<BookingDto?> GetBookingByIdAsync(Guid bookingId, Guid userId);
    Task<BookingDto?> GetBookingByIdForAdminAsync(Guid bookingId);
    Task<List<BookingDto>> GetBookingsByResourceIdAsync(Guid resourceId);
    Task<List<BookingDto>> GetAllBookingsAsync();

    // Series Operations
    Task<List<BookingDto>> GetBookingsBySeriesAsync(Guid recurrenceGroupId);
    Task<bool> UpdateSeriesTimeAsync(Guid recurrenceGroupId, TimeSpan timeShift);
    Task<bool> CancelSeriesAsync(Guid recurrenceGroupId, Guid userId);
}