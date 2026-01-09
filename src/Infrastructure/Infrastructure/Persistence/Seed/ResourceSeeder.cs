using Domain.Entities;
using Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed
{
    public static class ResourceSeeder
    {
        public static async Task SeedResourcesAsync(SystemRezerwacjiDbContext context)
        {
            // Sprawdź, czy w bazie istnieją już jakiekolwiek zasoby
            if (await context.Resources.AnyAsync())
            {
                return;
            }
            
            // Pobierz typy zasobów (zakładamy, że ResourceTypeSeeder uruchomił się wcześniej)
            var salaType = await context.ResourceTypes.FirstOrDefaultAsync(rt => rt.Name == "Sala Konferencyjna");
            var deskType = await context.ResourceTypes.FirstOrDefaultAsync(rt => rt.Name == "Biurko Hot Desk" || rt.Name == "Biurko");
            var sprzetType = await context.ResourceTypes.FirstOrDefaultAsync(rt => rt.Name == "Sprzęt");
            var parkingType = await context.ResourceTypes.FirstOrDefaultAsync(rt => rt.Name == "Parking");

            // Jeśli brakuje typów, stwórzmy je awaryjnie
            if (salaType == null) { salaType = new ResourceType { Name = "Sala Konferencyjna" }; context.ResourceTypes.Add(salaType); }
            if (deskType == null) { deskType = new ResourceType { Name = "Biurko" }; context.ResourceTypes.Add(deskType); }
            if (sprzetType == null) { sprzetType = new ResourceType { Name = "Sprzęt" }; context.ResourceTypes.Add(sprzetType); }
            if (parkingType == null) { parkingType = new ResourceType { Name = "Parking" }; context.ResourceTypes.Add(parkingType); }

            // Zapisz typy
            if (context.ChangeTracker.HasChanges()) await context.SaveChangesAsync();

            var resources = new List<Resource>
            {
                // Salki
                new Resource { Name = "Sala Matrix (Duża)", Description = "Sala na 12 osób z rzutnikiem 4K, system wideokonferencyjny", Capacity = 12, Location = "Piętro 1, Skrzydło A", ResourceTypeId = salaType.Id, ResourceType = salaType, IsActive = true },
                new Resource { Name = "Sala Zion (Mała)", Description = "Salka spotkań 1:1, wyciszona", Capacity = 3, Location = "Piętro 1, Obok kuchni", ResourceTypeId = salaType.Id, ResourceType = salaType, IsActive = true },
                new Resource { Name = "Sala Nebuchadnezzar", Description = "Kreatywna przestrzeń z pufami i tablicą ścieralną", Capacity = 8, Location = "Piętro 2", ResourceTypeId = salaType.Id, ResourceType = salaType, IsActive = true },
                
                // Biurka / Hot Desks
                new Resource { Name = "Hot Desk #100 (Cisza)", Description = "Biurko w strefie cichej", Capacity = 1, Location = "Open Space A", ResourceTypeId = deskType.Id, ResourceType = deskType, IsActive = true },
                new Resource { Name = "Hot Desk #101 (Okno)", Description = "Biurko przy oknie (Monitor Dell 27')", Capacity = 1, Location = "Open Space A", ResourceTypeId = deskType.Id, ResourceType = deskType, IsActive = true },
                new Resource { Name = "Hot Desk #102", Description = "Biurko standardowe", Capacity = 1, Location = "Open Space A", ResourceTypeId = deskType.Id, ResourceType = deskType, IsActive = true },
                new Resource { Name = "Stanowisko Pair Programming", Description = "2 monitory 4K, 1 komputer, szerokie biurko", Capacity = 2, Location = "Open Space B", ResourceTypeId = deskType.Id, ResourceType = deskType, IsActive = true },
                new Resource { Name = "Biurko Project Managera", Description = "Biurko dedykowane (Anna Nowak)", Capacity = 1, Location = "Biuro PM", ResourceTypeId = deskType.Id, ResourceType = deskType, IsActive = true },

                // Sprzęt
                new Resource { Name = "iPhone 15 Pro Max (Testowy)", Description = "Urządzenie testowe iOS 17, Sim do testów", Capacity = 1, Location = "Szafa QA", ResourceTypeId = sprzetType.Id, ResourceType = sprzetType, IsActive = true },
                new Resource { Name = "Samsung S24 Ultra", Description = "Urządzenie testowe Android 14", Capacity = 1, Location = "Szafa QA", ResourceTypeId = sprzetType.Id, ResourceType = sprzetType, IsActive = true },
                new Resource { Name = "Google Pixel 8", Description = "Czysty Android", Capacity = 1, Location = "Szafa QA", ResourceTypeId = sprzetType.Id, ResourceType = sprzetType, IsActive = true },
                new Resource { Name = "Oculus Quest 3", Description = "Gogle VR do testów aplikacji immersyjnych", Capacity = 1, Location = "Lab VR", ResourceTypeId = sprzetType.Id, ResourceType = sprzetType, IsActive = true },
                new Resource { Name = "MacBook Pro M3 (Zapasowy)", Description = "Laptop zastępczy (hasło w IT)", Capacity = 1, Location = "Dział IT", ResourceTypeId = sprzetType.Id, ResourceType = sprzetType, IsActive = true },
                
                // Parking
                new Resource { Name = "Miejsce Parkingowe 12 (Goście)", Description = "Miejsce przy wejściu głównym", Capacity = 1, Location = "Parking podziemny -1", ResourceTypeId = parkingType.Id, ResourceType = parkingType, IsActive = true },
                new Resource { Name = "Miejsce Parkingowe 15", Description = "Miejsce standardowe", Capacity = 1, Location = "Parking podziemny -1", ResourceTypeId = parkingType.Id, ResourceType = parkingType, IsActive = true }
            };

            context.Resources.AddRange(resources);
            await context.SaveChangesAsync();
        }
    }
}
