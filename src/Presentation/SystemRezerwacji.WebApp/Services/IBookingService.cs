using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemRezerwacji.WebApp.Models;

namespace SystemRezerwacji.WebApp.Services
{
    public interface IBookingService
    {
        Task CreateBookingAsync(BookingRequestDto dto);
        Task<List<BookingDto>> GetMyBookingsAsync();
        Task CancelBookingAsync(Guid bookingId);
    }
}