using Application.Interfaces.Booking;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Booking;

namespace Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly SystemRezerwacjiDbContext _context;

    public BookingService(SystemRezerwacjiDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookingDto>> GetUserBookingsAsync(Guid userId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId) // 1. Filtruj rezerwacje dla konkretnego użytkownika
            .Include(b => b.Resource)      // 2. Dołącz powiązany obiekt zasobu (dzięki temu mamy jego nazwę)
            .OrderByDescending(b => b.StartTime) // 3. Sortuj od najnowszych
            .Select(b => new BookingDto // 4. Mapuj na DTO, aby wysłać tylko potrzebne dane
            {
                Id = b.Id,
                ResourceId = b.ResourceId,
                UserId = b.UserId,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString(),
                ResourceName = b.Resource.Name, // Pobieramy nazwę z dołączonego zasobu
                UserName = "" // Na liście "moich" rezerwacji, nazwa użytkownika nie jest potrzebna
            })
            .ToListAsync(); // 5. Wykonaj zapytanie do bazy danych

        return bookings;
    }
    
      public async Task<BookingDto?> CreateBookingAsync(BookingRequestDto request, Guid userId)
    {
        // === ZMIANA: Dodajemy walidację na null ===
        if (request.StartTime is null || request.EndTime is null)
        {
            return null; // Nieprawidłowe żądanie
        }

        var isConflict = await _context.Bookings
            .AnyAsync(b => b.ResourceId == request.ResourceId &&
                           b.Status != BookingStatus.CancelledByUser &&
                           (request.StartTime.Value < b.EndTime && request.EndTime.Value > b.StartTime));

        if (isConflict)
        {
            return null;
        }

        var user = await _context.Users.FindAsync(userId);
        var resource = await _context.Resources.FindAsync(request.ResourceId);

        if (user is null || resource is null)
        {
            return null;
        }

        var newBooking = new Booking
        {
            ResourceId = request.ResourceId,
            UserId = userId,
            // === ZMIANA: Używamy. Value, aby pobrać datę z typu nullowalnego ===
            StartTime = request.StartTime.Value,
            EndTime = request.EndTime.Value,
            Status = BookingStatus.Confirmed,
            Notes = request.Notes,
            Resource = resource,
            User = user
        };

        _context.Bookings.Add(newBooking);
        await _context.SaveChangesAsync();
        
        // Zwracamy pełne DTO
        return new BookingDto
        {
            Id = newBooking.Id,
            ResourceId = newBooking.ResourceId,
            UserId = newBooking.UserId,
            StartTime = newBooking.StartTime,
            EndTime = newBooking.EndTime,
            Status = newBooking.Status.ToString(),
            ResourceName = resource.Name,
            UserName = user.UserName ?? user.Email
        };
    }

    public async Task<bool> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request, Guid userId)
    {
        var bookingToUpdate = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

        if (bookingToUpdate == null)
        {
            return false;
        }

        // === ZMIANA: Dodajemy walidację na null również tutaj ===
        if (request.StartTime == default || request.EndTime == default)
        {
            return false; // Nieprawidłowe daty
        }

        var isConflict = await _context.Bookings
            .AnyAsync(b => 
                b.Id != bookingId &&
                b.ResourceId == bookingToUpdate.ResourceId &&
                b.Status != BookingStatus.CancelledByUser &&
                (request.StartTime < b.EndTime && request.EndTime > b.StartTime)
            );

        if (isConflict)
        {
            return false;
        }

        bookingToUpdate.StartTime = request.StartTime;
        bookingToUpdate.EndTime = request.EndTime;
        bookingToUpdate.Notes = request.Notes;

        _context.Bookings.Update(bookingToUpdate);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<bool> CancelBookingAsync(Guid bookingId, Guid userId)
    {
        // Krok 1: Znajdź rezerwację, upewniając się, że należy do danego użytkownika
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

        if (booking == null)
        {
            // Rezerwacja nie istnieje lub użytkownik nie ma do niej dostępu.
            return false;
        }

        // Opcjonalnie: sprawdź, czy rezerwacja nie jest już w przeszłości
        if (booking.StartTime < DateTime.UtcNow)
        {
            // Można tu zaimplementować regułę biznesową, np.
            // "nie można anulować rezerwacji, która już się rozpoczęła".
            // Na razie tego nie robimy, ale to dobre miejsce na taką logikę.
        }

        // Krok 2: Zmień status i zapisz zmiany
        booking.Status = BookingStatus.CancelledByUser; // Używamy dedykowanego statusu
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<BookingDto?> GetBookingByIdAsync(Guid bookingId, Guid userId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Resource)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

        if (booking == null)
        {
            return null;
        }

        return new BookingDto
        {
            Id = booking.Id,
            ResourceId = booking.ResourceId,
            UserId = booking.UserId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Status = booking.Status.ToString(),
            ResourceName = booking.Resource.Name,
            UserName = booking.User.UserName ?? booking.User.Email // Tutaj nazwa użytkownika ma sens
        };
    }
    
    
}