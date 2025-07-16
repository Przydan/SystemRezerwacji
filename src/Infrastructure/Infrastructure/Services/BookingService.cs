using Application.Interfaces.Booking;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.DTOs.Booking;

namespace Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly SystemRezerwacjiDbContext _context;
        private readonly ILogger<BookingService> _logger;
        
        public BookingService(SystemRezerwacjiDbContext context, ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        

        public async Task<BookingDto?> CreateBookingAsync(BookingRequestDto request, Guid userId)
        {
            _logger.LogInformation("--- Rozpoczynanie tworzenia rezerwacji dla zasobu: {ResourceId} ---", request.ResourceId);
            _logger.LogInformation("Nowy termin: Od {StartTime} do {EndTime}", request.StartTime, request.EndTime);

            if (request.StartTime is null || request.EndTime is null)
            {
                _logger.LogError("Błąd walidacji: StartTime lub EndTime jest nullem.");
                return null;
            }
            
            var existingBookingsForResource = await _context.Bookings
                .Where(b => b.ResourceId == request.ResourceId)
                .ToListAsync();

            if (!existingBookingsForResource.Any())
            {
                _logger.LogWarning("DIAGNOSTYKA: Baza danych nie zwróciła ŻADNYCH rezerwacji dla zasobu {ResourceId}. Tabela jest pusta lub nie ma rezerwacji dla tego zasobu.", request.ResourceId);
            }
            else
            {
                _logger.LogDebug("Znaleziono {Count} istniejących rezerwacji dla zasobu {ResourceId}.", existingBookingsForResource.Count, request.ResourceId);
                foreach (var existing in existingBookingsForResource)
                {
                    _logger.LogWarning("- Istniejąca rezerwacja ID: {BookingId}, Termin: Od {StartTime} do {EndTime}, Status: {Status}", existing.Id, existing.StartTime, existing.EndTime, existing.Status);
                }
            }

            var isConflict = await _context.Bookings
                .AnyAsync(b => b.ResourceId == request.ResourceId &&
                               b.Status != BookingStatus.CancelledByUser &&
                               (request.StartTime.Value < b.EndTime && request.EndTime.Value > b.StartTime));

            _logger.LogInformation("Wynik sprawdzenia konfliktu (isConflict): {IsConflict}", isConflict);

            if (isConflict)
            {
                _logger.LogError("!!! WYKRYTO KONFLIKT. Zwracam błąd 409. !!!");
                return null;
            }

            var user = await _context.Users.FindAsync(userId);
            var resource = await _context.Resources.FindAsync(request.ResourceId);

            if (user is null || resource is null)
            {
                 _logger.LogError("Nie znaleziono użytkownika (ID: {UserId}) lub zasobu (ID: {ResourceId})", userId, request.ResourceId);
                return null;
            }

            var newBooking = new Booking
            {
                ResourceId = request.ResourceId,
                UserId = userId,
                StartTime = request.StartTime.Value,
                EndTime = request.EndTime.Value,
                Status = BookingStatus.Confirmed,
                Notes = request.Notes,
                Resource = resource,
                User = user
            };

            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Rezerwacja pomyślnie zapisana w bazie danych. Nowe ID: {BookingId}", newBooking.Id);

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

        public Task<BookingDto?> CreateBookingAsync(BookingRequestDto bookingRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BookingDto>> GetUserBookingsAsync(Guid userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Resource)
                .OrderByDescending(b => b.StartTime)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    ResourceId = b.ResourceId,
                    UserId = b.UserId,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    ResourceName = b.Resource.Name,
                    UserName = ""
                })
                .ToListAsync();
            return bookings;
        }

        public async Task<bool> AdminCancelBookingAsync(Guid bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                _logger.LogWarning("Admin próbował usunąć nieistniejącą rezerwację o ID: {BookingId}", bookingId);
                return false;
            }

            // Admin usuwa rezerwację - można ustawić inny status dla odróżnienia
            booking.Status = BookingStatus.CancelledByAdmin; 
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Admin usunął rezerwację o ID: {BookingId}", bookingId);
            return true;
        }

        public async Task<bool> UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request, Guid userId)
        {
            var bookingToUpdate = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (bookingToUpdate == null) return false;
            if (request.StartTime == default || request.EndTime == default) return false;

            var isConflict = await _context.Bookings
                .AnyAsync(b => 
                    b.Id != bookingId &&
                    b.ResourceId == bookingToUpdate.ResourceId &&
                    b.Status != BookingStatus.CancelledByUser &&
                    (request.StartTime < b.EndTime && request.EndTime > b.StartTime)
                );

            if (isConflict) return false;

            bookingToUpdate.StartTime = request.StartTime;
            bookingToUpdate.EndTime = request.EndTime;
            bookingToUpdate.Notes = request.Notes;

            _context.Bookings.Update(bookingToUpdate);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> CancelBookingAsync(Guid bookingId, Guid userId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);
            if (booking == null) return false;

            booking.Status = BookingStatus.CancelledByUser;
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
            if (booking == null) return null;

            return new BookingDto
            {
                Id = booking.Id,
                ResourceId = booking.ResourceId,
                UserId = booking.UserId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Status = booking.Status.ToString(),
                ResourceName = booking.Resource.Name,
                UserName = booking.User.UserName ?? booking.User.Email
            };
        }
        
        public Task<List<BookingDto>?> GetBookingsForUserAsync(Guid userId)
        {
            return Task.FromResult<List<BookingDto>>(null);
        }
    }
}
