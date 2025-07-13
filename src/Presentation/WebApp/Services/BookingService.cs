// using System.Net.Http.Json;
// using Shared.DTOs.Booking;
//
// namespace WebApp.Services;
//
// public class BookingService : IBookingService
// {
//     private readonly HttpClient _httpClient;
//
//     public BookingService(HttpClient httpClient)
//     {
//         _httpClient = httpClient;
//     }
//
//     public async Task<List<BookingDto>?> GetMyBookingsAsync()
//     {
//         try
//         {
//             return await _httpClient.GetFromJsonAsync<List<BookingDto>>("api/bookings/my");
//         }
//         catch { return null; }
//     }
//
//     public async Task<BookingDto?> GetBookingByIdAsync(Guid bookingId)
//     {
//         try
//         {
//             return await _httpClient.GetFromJsonAsync<BookingDto>($"api/bookings/{bookingId}");
//         }
//         catch { return null; }
//     }
//
//     public async Task<BookingDto?> CreateBookingAsync(BookingRequestDto request)
//     {
//         var response = await _httpClient.PostAsJsonAsync("api/bookings", request);
//         if (!response.IsSuccessStatusCode) return null;
//         return await response.Content.ReadFromJsonAsync<BookingDto>();
//     }
//
//     public async Task<bool> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request)
//     {
//         var response = await _httpClient.PutAsJsonAsync($"api/bookings/{bookingId}", request);
//         return response.IsSuccessStatusCode;
//     }
//
//     public async Task<bool> CancelBookingAsync(Guid bookingId)
//     {
//         var response = await _httpClient.DeleteAsync($"api/bookings/{bookingId}");
//         return response.IsSuccessStatusCode;
//     }
//     
//     public Task<BookingDto?> GetBookingByIdAsync(Guid id)
//     {
//         return _httpClient.GetFromJsonAsync<BookingDto?>($"/api/bookings/{id}");
//     }
// }