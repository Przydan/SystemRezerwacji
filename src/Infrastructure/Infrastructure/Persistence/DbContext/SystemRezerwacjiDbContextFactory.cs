using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
// Microsoft.Extensions.Configuration i System.IO nie są potrzebne dla tego uproszczonego podejścia

namespace Infrastructure.Persistence.DbContext
{
    public class SystemRezerwacjiDbContextFactory : IDesignTimeDbContextFactory<SystemRezerwacjiDbContext>
    {
        public SystemRezerwacjiDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SystemRezerwacjiDbContext>();

            // Użyj tego samego typu bazy i podobnej nazwy pliku jak w appsettings.json projektu Server,
            // aby zachować spójność, ale pamiętaj, że to jest tylko dla narzędzi EF Core.
            // Plik bazy danych zostanie utworzony względem katalogu, z którego uruchamiasz `dotnet ef`.
            // Jeśli uruchamiasz z głównego folderu solucji, plik powstanie tam.
            var connectionString = "Data Source=systemrezerwacji_design_time.db";

            optionsBuilder.UseSqlite(connectionString);

            return new SystemRezerwacjiDbContext(optionsBuilder.Options);
        }
    }
}