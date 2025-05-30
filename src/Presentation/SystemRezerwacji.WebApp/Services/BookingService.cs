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
            var response = await _httpClient.PostAsJsonAsync("api/bookings", dto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"CreateBooking failed: {response.StatusCode} – {error}");
            }
        }

        public async Task<List<BookingDto>> GetMyBookingsAsync()
        {
            var bookings = await _httpClient.GetFromJsonAsync<List<BookingDto>>("api/bookings/my");
            return bookings ?? new List<BookingDto>();
        }

        public async Task CancelBookingAsync(Guid bookingId)
        {
            var response = await _httpClient.PostAsync($"api/bookings/{bookingId}/cancel", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"CancelBooking failed: {response.StatusCode} – {error}");
            }
        }
    }
}