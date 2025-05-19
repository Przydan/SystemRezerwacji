using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemRezerwacji.ClientApp.Models;

namespace SystemRezerwacji.ClientApp.Services
{
    public class BookingService
    {
        private readonly HttpClient _httpClient;

        public BookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateBookingAsync(CreateBookingDto dto)
        {
            // Zakładamy, że API posiada endpoint POST /api/bookings do tworzenia rezerwacji
            var response = await _httpClient.PostAsJsonAsync("api/bookings", dto);
            if (!response.IsSuccessStatusCode)
            {
                // Odczytaj ewentualną wiadomość błędu z odpowiedzi
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"CreateBooking failed: {response.ReasonPhrase} - {error}");
            }
        }

        public async Task<List<BookingDto>> GetMyBookingsAsync()
        {
            // Zakładamy endpoint GET /api/bookings/my zwracający rezerwacje użytkownika
            var bookings = await _httpClient.GetFromJsonAsync<List<BookingDto>>("api/bookings/my");
            return bookings ?? new List<BookingDto>();
        }

        public async Task CancelBookingAsync(Guid bookingId)
        {
            // Zakładamy endpoint DELETE /api/bookings/{id} dokonujący "anulowania" (zmiany statusu) rezerwacji
            var response = await _httpClient.DeleteAsync($"api/bookings/{bookingId}");
            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"CancelBooking failed: {response.ReasonPhrase} - {error}");
            }
        }
    }
}