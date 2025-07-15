using System.Net.Http.Json;
using Application.Interfaces.Booking;
using Shared.DTOs.Booking;

namespace WebApp.Services;

public class BookingService : IBookingService
{
    private readonly HttpClient _httpClient;

    public BookingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BookingDto>> GetUserBookingsAsync(Guid userId)
    {
        // Poprawiony endpoint na "my" zamiast "my-bookings"
        var bookings = await _httpClient.GetFromJsonAsync<List<BookingDto>>("api/bookings/my");
        return bookings ?? new List<BookingDto>();
    }

    public async Task<BookingDto?> CreateBookingAsync(BookingRequestDto bookingRequest, Guid userId)
    {
        // userId nie jest potrzebne w ciele żądania, serwer pobierze je z tokenu.
        var response = await _httpClient.PostAsJsonAsync("api/bookings", bookingRequest);
        if (!response.IsSuccessStatusCode) return null;
        
        return await response.Content.ReadFromJsonAsync<BookingDto>();
    }

    public async Task<bool> CancelBookingAsync(Guid bookingId, Guid userId)
    {
        // userId nie jest potrzebne, serwer zweryfikuje je na podstawie tokenu.
        var response = await _httpClient.DeleteAsync($"api/bookings/{bookingId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request, Guid userId)
    {
        // userId nie jest potrzebne, serwer zweryfikuje je na podstawie tokenu.
        var response = await _httpClient.PutAsJsonAsync($"api/bookings/{bookingId}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<BookingDto?> GetBookingByIdAsync(Guid bookingId, Guid userId)
    {
        // userId nie jest potrzebne, serwer zweryfikuje je na podstawie tokenu.
        return await _httpClient.GetFromJsonAsync<BookingDto>($"api/bookings/{bookingId}");
    }
}