using Microsoft.EntityFrameworkCore; // Potrzebne dla AnyAsync i AddAsync
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Entities;
// Upewnij się, że ta ścieżka jest poprawna
using System; // Potrzebne dla Guid
using System.Linq; // Potrzebne dla AnyAsync
using System.Threading.Tasks;
using Infrastructure.Persistence.DbContext; // Potrzebne dla Task

namespace Infrastructure.Persistence.Seed;

public class ResourceTypeSeeder
{
    public static async Task SeedResourceTypeAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<ResourceTypeSeeder>>();
        // Pobieramy DbContext z kontenera DI w ramach scope'u
        // W metodzie SeedDevelopmentData w Program.cs już tworzysz scope, więc to jest OK.
        var context = services.GetRequiredService<SystemRezerwacjiDbContext>();

        try
        {
            // Sprawdź, czy jakiekolwiek typy zasobów już istnieją
            if (await context.ResourceTypes.AnyAsync())
            {
                logger.LogInformation("Baza danych zawiera już typy zasobów. Pomijam seedowanie ResourceType.");
                return; // Dane już istnieją, nie ma potrzeby seedowania
            }

            logger.LogInformation("Rozpoczynam seedowanie domyślnych typów zasobów (ResourceType).");

            var resourceTypes = new[]
            {
                new ResourceType
                {
                    Id = Guid.NewGuid(), Name = "Sala Konferencyjna",
                    Description = "Pomieszczenie do spotkań biznesowych i prezentacji.", IconCssClass = "fas fa-users"
                },
                new ResourceType
                {
                    Id = Guid.NewGuid(), Name = "Sprzęt IT", Description = "Rzutniki, laptopy, monitory, itp.",
                    IconCssClass = "fas fa-laptop-code"
                },
                new ResourceType
                {
                    Id = Guid.NewGuid(), Name = "Pokój Gościnny", Description = "Komfortowy pokój dla gości firmy.",
                    IconCssClass = "fas fa-bed"
                },
                new ResourceType
                {
                    Id = Guid.NewGuid(), Name = "Samochód Służbowy", Description = "Pojazd do użytku służbowego.",
                    IconCssClass = "fas fa-car"
                },
                new ResourceType
                {
                    Id = Guid.NewGuid(), Name = "Biurko Hot Desk",
                    Description = "Stanowisko pracy do wynajęcia na godziny/dni.", IconCssClass = "fas fa-desktop"
                }
            };

            await context.ResourceTypes.AddRangeAsync(resourceTypes);
            await context.SaveChangesAsync();

            logger.LogInformation("Seedowanie typów zasobów (ResourceType) zakończone pomyślnie.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Wystąpił błąd podczas seedowania danych ResourceType.");
            // Rozważ rzucenie wyjątku dalej, jeśli błąd seedowania jest krytyczny dla startu aplikacji
            // throw;
        }
    }
}