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
        private readonly Application.Interfaces.Infrastructure.IEmailService _emailService;
        
        public BookingService(SystemRezerwacjiDbContext context, ILogger<BookingService> logger, Application.Interfaces.Infrastructure.IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }
        
        

        public async Task<BookingDto?> CreateBookingAsync(BookingRequestDto request, Guid userId)
        {
            _logger.LogInformation("--- Tworzenie rezerwacji (Refactored) --- Zasób: {ResourceId}", request.ResourceId);

            if (request.StartTime is null || request.EndTime is null)
            {
                _logger.LogError("Błąd walidacji: StartTime/EndTime null.");
                return null;
            }

            var user = await _context.Users.FindAsync(userId);
            if (user is null) throw new UnauthorizedAccessException("Użytkownik nie istnieje.");

            var resource = await _context.Resources.FindAsync(request.ResourceId);
            if (resource is null) throw new KeyNotFoundException("Zasób nie istnieje.");

            // Calculate Slots
            var slots = CalculateRecurrenceSlots(request);
            var bookingsToCreate = new List<Booking>();
            Guid? recurrenceGroupId = request.IsRecurring ? Guid.NewGuid() : null;

            int cycleIndex = 1;
            foreach (var slot in slots)
            {
                if (await IsSlotConflictAsync(request.ResourceId, slot.Start, slot.End))
                {
                     _logger.LogWarning("Konflikt w cyklu {Cycle} ({Start}). Przerwano.", cycleIndex, slot.Start);
                     return null;
                }

                bookingsToCreate.Add(new Booking
                {
                    ResourceId = request.ResourceId,
                    UserId = userId,
                    StartTime = slot.Start,
                    EndTime = slot.End,
                    Status = BookingStatus.Confirmed,
                    Notes = request.Notes + (request.IsRecurring ? $" (Cykl {cycleIndex}/{slots.Count})" : ""),
                    Resource = resource,
                    User = user,
                    RecurrenceGroupId = recurrenceGroupId
                });
                cycleIndex++;
            }

            _context.Bookings.AddRange(bookingsToCreate);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Zapisano {Count} rezerwacji.", bookingsToCreate.Count);

            // Email Notification (Summary)
            var first = bookingsToCreate.First();
            await SendConfirmationEmailAsync(user, resource, first, bookingsToCreate.Count, request.IsRecurring ? request.Frequency : null);

            return new BookingDto
            {
                Id = first.Id,
                ResourceId = first.ResourceId,
                UserId = first.UserId,
                StartTime = first.StartTime,
                EndTime = first.EndTime,
                Status = first.Status.ToString(),
                ResourceName = resource.Name,
                UserName = user.UserName ?? user.Email
            };
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

            if (await IsSlotConflictAsync(bookingToUpdate.ResourceId, request.StartTime, request.EndTime, bookingId))
                return false;

            bookingToUpdate.StartTime = request.StartTime;
            bookingToUpdate.EndTime = request.EndTime;
            bookingToUpdate.Notes = request.Notes;

            _context.Bookings.Update(bookingToUpdate);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> AdminUpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request)
        {
            var bookingToUpdate = await _context.Bookings.FindAsync(bookingId);

            if (bookingToUpdate == null) return false;
            if (request.StartTime == default || request.EndTime == default) return false;

            if (await IsSlotConflictAsync(bookingToUpdate.ResourceId, request.StartTime, request.EndTime, bookingId))
                return false;

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
        
        public async Task<BookingDto?> GetBookingByIdForAdminAsync(Guid bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
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
                UserName = booking.User.UserName ?? booking.User.Email,
                Notes = booking.Notes
            };
        }
        
        public Task<List<BookingDto>?> GetBookingsForUserAsync(Guid userId)
        {
            return Task.FromResult<List<BookingDto>?>(null);
        }

        public async Task<List<BookingDto>> GetBookingsByResourceIdAsync(Guid resourceId)
        {
            return await _context.Bookings
                .Where(b => b.ResourceId == resourceId && b.EndTime >= DateTime.UtcNow) // Pokaż tylko przyszłe i aktywne rezerwacje
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    ResourceId = b.ResourceId,
                    UserId = b.UserId,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Notes = b.Notes
                })
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<List<BookingDto>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Resource)
                .Include(b => b.User)
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
                    UserName = b.User.UserName ?? b.User.Email,
                    Notes = b.Notes
                })
                .ToListAsync();
        }
    
        // --- HELPER METHODS ---

        private async Task<bool> IsSlotConflictAsync(Guid resourceId, DateTime start, DateTime end, Guid? excludeBookingId = null)
        {
            return await _context.Bookings
                .AnyAsync(b => b.ResourceId == resourceId &&
                               (excludeBookingId == null || b.Id != excludeBookingId) &&
                               b.Status != BookingStatus.CancelledByUser &&
                               b.Status != BookingStatus.CancelledByAdmin &&
                               (start < b.EndTime && end > b.StartTime));
        }

        private List<(DateTime Start, DateTime End)> CalculateRecurrenceSlots(BookingRequestDto request)
        {
            var slots = new List<(DateTime Start, DateTime End)>();
            int occurrences = 1;

            if (request.IsRecurring && request.Occurrences.HasValue && request.Occurrences.Value > 1)
            {
                occurrences = Math.Min(request.Occurrences.Value, 20); // Safety limit
            }

            for (int i = 0; i < occurrences; i++)
            {
                var start = request.StartTime!.Value;
                var end = request.EndTime!.Value;

                if (request.IsRecurring && i > 0)
                {
                    if (request.Frequency == "Weekly")
                    {
                        start = start.AddDays(7 * i);
                        end = end.AddDays(7 * i);
                    }
                    else // Daily
                    {
                        start = start.AddDays(i);
                        end = end.AddDays(i);
                    }
                }
                slots.Add((start, end));
            }
            return slots;
        }

        private async Task SendConfirmationEmailAsync(User user, Resource resource, Booking firstBooking, int count, string? frequency)
        {
             var emailBody = $"Witaj {user.UserName ?? user.Email},\n\n" +
                            $"Twoja rezerwacja została potwierdzona.\n" +
                            $"Zasób: {resource.Name}\n" +
                            (count > 1 ? $"Seria: {count} spotkań ({frequency})\n" : "") +
                            $"Pierwszy termin: {firstBooking.StartTime} - {firstBooking.EndTime}\n" +
                            $"Uwagi: {firstBooking.Notes}\n\n" +
                            $"Dziękujemy za korzystanie z systemu.";
                            
            await _emailService.SendEmailAsync(user.Email ?? "user@example.com", "Potwierdzenie Rezerwacji", emailBody);
        }

        // --- SERIES OPERATIONS ---

        public async Task<List<BookingDto>> GetBookingsBySeriesAsync(Guid recurrenceGroupId)
        {
            return await _context.Bookings
                .Where(b => b.RecurrenceGroupId == recurrenceGroupId)
                .Include(b => b.Resource)
                .Include(b => b.User)
                .OrderBy(b => b.StartTime)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    ResourceId = b.ResourceId,
                    UserId = b.UserId,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    ResourceName = b.Resource.Name,
                    UserName = b.User.UserName ?? b.User.Email,
                    Notes = b.Notes,
                    RecurrenceGroupId = b.RecurrenceGroupId
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateSeriesTimeAsync(Guid recurrenceGroupId, TimeSpan timeShift)
        {
            var bookingsInSeries = await _context.Bookings
                .Where(b => b.RecurrenceGroupId == recurrenceGroupId && b.Status == BookingStatus.Confirmed)
                .ToListAsync();

            if (!bookingsInSeries.Any()) return false;

            // Check conflicts for all shifted slots
            foreach (var booking in bookingsInSeries)
            {
                var newStart = booking.StartTime.Add(timeShift);
                var newEnd = booking.EndTime.Add(timeShift);

                if (await IsSlotConflictAsync(booking.ResourceId, newStart, newEnd, booking.Id))
                {
                    _logger.LogWarning("Conflict detected when shifting series for booking {Id}", booking.Id);
                    return false; // Fail entire operation
                }
            }

            // Apply shifts
            foreach (var booking in bookingsInSeries)
            {
                booking.StartTime = booking.StartTime.Add(timeShift);
                booking.EndTime = booking.EndTime.Add(timeShift);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Shifted {Count} bookings in series by {Shift}", bookingsInSeries.Count, timeShift);
            return true;
        }

        public async Task<bool> CancelSeriesAsync(Guid recurrenceGroupId, Guid userId)
        {
            var bookingsInSeries = await _context.Bookings
                .Where(b => b.RecurrenceGroupId == recurrenceGroupId && b.UserId == userId && b.Status == BookingStatus.Confirmed)
                .ToListAsync();

            if (!bookingsInSeries.Any()) return false;

            foreach (var booking in bookingsInSeries)
            {
                booking.Status = BookingStatus.CancelledByUser;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Cancelled {Count} bookings in series", bookingsInSeries.Count);
            return true;
        }
    }
}
