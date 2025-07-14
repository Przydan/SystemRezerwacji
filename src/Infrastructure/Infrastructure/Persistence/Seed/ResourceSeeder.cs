using Domain.Entities;
using Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seed
{
    public static class ResourceSeeder
    {
        public static async Task SeedAsync(SystemRezerwacjiDbContext context)
        {
            // Sprawdź, czy w bazie istnieją już jakiekolwiek zasoby
            if (await context.Resources.AnyAsync())
            {
                return; // Jeśli tak, zakończ działanie
            }
            
            // Najpierw pobierz istniejący typ zasobu "Sala Konferencyjna"
            // Używamy FirstOrDefaultAsync dla operacji asynchronicznej
            var roomType = await context.ResourceTypes.FirstOrDefaultAsync(rt => rt.Name == "Sala Konferencyjna");
            if (roomType == null)
            {
                return;
            }
            
            // Pobierzmy też inny typ dla różnorodności
            var deskType = await context.ResourceTypes.FirstOrDefaultAsync(rt => rt.Name == "Biurko Hot Desk");
            if (deskType == null)
            {
                 return;
            }

            var resources = new List<Resource>
            {
                new Resource
                {
                    // Użyjmy tego samego ID, co w Twoim teście, abyś mógł od razu go użyć!
                    Id = Guid.Parse("28920341-70d0-4aa3-9fa9-7f66fedcf2cb"),
                    Name = "Sala 'Słoneczna'",
                    Description = "Duża sala konferencyjna z projektorem.",
                    ResourceTypeId = roomType.Id,
                    ResourceType = null // Przypisujemy ID pobranego typu
                },
                new Resource
                {
                    Id = Guid.NewGuid(),
                    Name = "Sala 'Księżycowa'",
                    Description = "Mała sala do spotkań 1-na-1.",
                    ResourceTypeId = roomType.Id,
                    ResourceType = null
                },
                new Resource
                {
                    Id = Guid.NewGuid(),
                    Name = "Biurko A1",
                    Description = "Biurko przy oknie.",
                    ResourceTypeId = deskType.Id,
                    ResourceType = null
                }
            };

            await context.Resources.AddRangeAsync(resources);
            await context.SaveChangesAsync();
        }
    }
}
