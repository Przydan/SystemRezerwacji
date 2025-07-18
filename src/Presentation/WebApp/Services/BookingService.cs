using System.Net.Http.Json;
using Application.Interfaces.Booking;
using Shared.DTOs.Booking;
using WebApp.Interfaces;

namespace WebApp.Services;

public class BookingService : IWebAppBookingService
{
    private readonly HttpClient _httpClient;

    public BookingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<List<BookingDto>> GetMyBookingsAsync()
    {
        var bookings = await _httpClient.GetFromJsonAsync<List<BookingDto>>("api/bookings/my");
        return bookings ?? new List<BookingDto>();
    }

    public async Task<BookingDto?> CreateBookingAsync(BookingRequestDto bookingRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("api/bookings", bookingRequest);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<BookingDto>() : null;
    }

    public async Task<bool> CancelBookingAsync(Guid bookingId)
    {
        var response = await _httpClient.DeleteAsync($"api/bookings/{bookingId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/bookings/{bookingId}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<BookingDto?> GetBookingByIdAsync(Guid bookingId)
    {
        return await _httpClient.GetFromJsonAsync<BookingDto>($"api/bookings/{bookingId}");
    }

    public async Task<bool> AdminCancelBookingAsync(Guid bookingId)
    {
        var response = await _httpClient.DeleteAsync($"api/bookings/admin/{bookingId}");
        return response.IsSuccessStatusCode;
    }
    
    public async Task<List<BookingDto>?> GetBookingsForUserAsync(Guid userId)
    {
        return await _httpClient.GetFromJsonAsync<List<BookingDto>>($"api/bookings/user/{userId}");
    }
}