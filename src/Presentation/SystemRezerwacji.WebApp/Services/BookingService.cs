using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SystemRezerwacji.WebApp.Models;

namespace SystemRezerwacji.WebApp.Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _httpClient;

        public BookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateBookingAsync(BookingRequestDto dto)
        {
            var resp = await _httpClient.PostAsJsonAsync("api/bookings", dto);
            resp.EnsureSuccessStatusCode();
        }

        public async Task<List<BookingDto>> GetMyBookingsAsync()
        {
            var list = await _httpClient.GetFromJsonAsync<List<BookingDto>>("api/bookings/my");
            return list ?? new List<BookingDto>();
        }

        public async Task UpdateBookingAsync(BookingRequestDto dto)
        {
            var resp = await _httpClient.PutAsJsonAsync($"api/bookings/{dto.Id}", dto);
            resp.EnsureSuccessStatusCode();
        }

        public async Task CancelBookingAsync(Guid bookingId)
        {
            var resp = await _httpClient.PostAsync($"api/bookings/{bookingId}/cancel", null);
            resp.EnsureSuccessStatusCode();
        }
    }
}