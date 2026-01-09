using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seed;

public class BookingSeeder
{
    public static async Task SeedBookingsAsync(SystemRezerwacjiDbContext context, ILogger logger)
    {
        try
        {
            if (await context.Bookings.CountAsync() > 50) 
            {
                logger.LogInformation("Rezerwacje już istnieją (powyżej 50). Pomijam seedowanie aby uniknąć duplikatów.");
                return;
            }

            var users = await context.Users.ToListAsync();
            var resources = await context.Resources.Include(r => r.ResourceType).Where(r => r.IsActive).ToListAsync();

            if (!users.Any() || !resources.Any()) return;

            var bookings = new List<Booking>();
            var startDate = DateTime.Today; // Start from today
            var random = new Random();

            // Helpers to find specific users/resources
            var qaUsers = users.Where(u => u.FirstName == "Piotr" || (u.LastName?.Contains("QA") == true)).ToList();
            var devUsers = users.Where(u => u.FirstName == "Jan" || u.FirstName == "Tomasz" || (u.LastName?.Contains("Dev") == true)).ToList();
            var pmUser = users.FirstOrDefault(u => u.FirstName == "Anna"); // PM
            
            var matrixRoom = resources.FirstOrDefault(r => r.Name.Contains("Matrix"));
            var zionRoom = resources.FirstOrDefault(r => r.Name.Contains("Zion"));
            var testDevices = resources.Where(r => r.ResourceType?.Name == "Sprzęt").ToList();
            var desks = resources.Where(r => r.ResourceType?.Name?.Contains("Biurko") == true).ToList();

            // Scenario 1: Daily Standups (Matrix Room, 09:30 - 10:00, Mon-Fri)
            if (matrixRoom != null && pmUser != null)
            {
                for (int d = 0; d < 30; d++) // Next 30 days
                {
                    var date = startDate.AddDays(d);
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        bookings.Add(new Booking
                        {
                            ResourceId = matrixRoom.Id,
                            UserId = pmUser.Id, // PM bookings standup
                            StartTime = date.AddHours(9).AddMinutes(30),
                            EndTime = date.AddHours(10),
                            Status = BookingStatus.Confirmed,
                            Notes = "Daily Standup - Zespół A",
                            Resource = matrixRoom,
                            User = pmUser
                        });
                    }
                }
            }

            // Scenario 2: QA Device Testing (Long bookings, 1-3 days)
            if (testDevices.Any() && qaUsers.Any())
            {
                foreach (var device in testDevices)
                {
                    // 2 bookings per device in near future
                    var qaUser = qaUsers[random.Next(qaUsers.Count)];
                    var start = startDate.AddDays(random.Next(1, 5));
                    
                    bookings.Add(new Booking
                    {
                        ResourceId = device.Id,
                        UserId = qaUser.Id,
                        StartTime = start.AddHours(9),
                        EndTime = start.AddDays(2).AddHours(17), // 2 days test
                        Status = BookingStatus.Confirmed,
                        Notes = "Testy regresyjne aplikacji mobilnej v2.0",
                        Resource = device,
                        User = qaUser
                    });
                }
            }

            // Scenario 3: PM Client Meetings (Zion Room, random slots)
            if (zionRoom != null && pmUser != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    var day = startDate.AddDays(random.Next(1, 14));
                    if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday) continue;
                    
                    bookings.Add(new Booking
                    {
                        ResourceId = zionRoom.Id,
                        UserId = pmUser.Id,
                        StartTime = day.AddHours(13),
                        EndTime = day.AddHours(14), // 1h
                        Status = BookingStatus.Confirmed,
                        Notes = "Call z klientem (USA)",
                        Resource = zionRoom,
                        User = pmUser
                    });
                }
            }

            // Scenario 4: Random Desk Bookings for Devs
            if (desks.Any() && devUsers.Any())
            {
                foreach (var dev in devUsers)
                {
                    for (int k = 0; k < 3; k++) // Each dev books desk 3 times
                    {
                        var desk = desks[random.Next(desks.Count)];
                        var day = startDate.AddDays(random.Next(0, 7));
                         if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday) continue;

                        // Check conflict
                        var start = day.AddHours(8);
                        var end = day.AddHours(16);
                        
                        if (!bookings.Any(b => b.ResourceId == desk.Id && b.StartTime < end && b.EndTime > start))
                        {
                            bookings.Add(new Booking
                            {
                                ResourceId = desk.Id,
                                UserId = dev.Id,
                                StartTime = start,
                                EndTime = end,
                                Status = BookingStatus.Confirmed,
                                Notes = "Praca stacjonarna",
                                Resource = desk,
                                User = dev
                            });
                        }
                    }
                }
            }
            
            // Add bookings to context
            if (bookings.Any())
            {
                context.Bookings.AddRange(bookings);
                await context.SaveChangesAsync();
                logger.LogInformation($"Zaseedowano {bookings.Count} realistycznych rezerwacji.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Błąd podczas seedowania rezerwacji.");
        }
    }
}
