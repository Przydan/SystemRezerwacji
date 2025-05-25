using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SystemRezerwacji.Domain.Entities;

namespace SystemRezerwacji.Infrastructure.Persistence.Seed;

public class ResourceTypeSeeder
{
     public static async Task SeedResourceTypeAsync(IServiceProvider services)
     {
         ModelBuilder mb = new ModelBuilder();
            
            var logger = services.GetRequiredService<ILogger<ResourceTypeSeeder>>();
            try
            {
                mb.Entity<ResourceType>().HasData(
                    new ResourceType
                        { Id = Guid.NewGuid(), Name = "Sala Konferencyjna", Description = "Pomieszczenie do spotkań" },
                    new ResourceType
                        { Id = Guid.NewGuid(), Name = "Sprzęt IT", Description = "Rzutniki, laptopy, etc." },
                    new ResourceType
                        { Id = Guid.NewGuid(), Name = "Pokój Gościnny", Description = "Pokój dla gości firmy" }
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Wystąpił błąd podczas seedowania danych ResourceType");
            }
        }
}