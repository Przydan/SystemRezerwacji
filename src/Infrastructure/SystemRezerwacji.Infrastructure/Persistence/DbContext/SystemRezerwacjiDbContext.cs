using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SystemRezerwacji.Domain.Entities;

namespace SystemRezerwacji.Infrastructure.Persistence.DbContext;

public class SystemRezerwacjiDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public SystemRezerwacjiDbContext(DbContextOptions<SystemRezerwacjiDbContext> options)
        : base(options)
    {
    }
    
    // DbSet dla każdej z Twoich głównych encji
    public DbSet<Resource> Resources { get; set; }
    public DbSet<ResourceType> ResourceTypes { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<ResourceFeature> ResourceFeatures { get; set; }
    // Encja User jest już zarządzana przez IdentityDbContext (jako DbSet<User>)


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Niezbędne przy dziedziczeniu z IdentityDbContext

        // Tutaj będziemy dodawać konfigurację Fluent API dla naszych encji
        // (relacje, klucze, ograniczenia, indeksy, seeding danych)

        // Przykład podstawowej konfiguracji dla Resource (rozwiniemy to):
        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            // ... więcej konfiguracji dla Resource ...

            // Definicja relacji Resource 1 --- * ResourceType
            entity.HasOne(d => d.ResourceType)
                .WithMany(p => p.Resources)
                .HasForeignKey(d => d.ResourceTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Zapobiega usunięciu typu, jeśli są do niego przypisane zasoby
        });

        // Przykład konfiguracji dla tabeli łączącej ResourceFeature
        modelBuilder.Entity<ResourceFeature>(entity =>
        {
            // Definicja klucza złożonego
            entity.HasKey(rf => new { rf.ResourceId, rf.FeatureId });

            // Relacja do Resource
            entity.HasOne(rf => rf.Resource)
                .WithMany(r =>
                    r.ResourceFeatures) // Upewnij się, że Resource ma ICollection<ResourceFeature> ResourceFeatures
                .HasForeignKey(rf => rf.ResourceId);

            // Relacja do Feature
            entity.HasOne(rf => rf.Feature)
                .WithMany(f =>
                    f.ResourceFeatures) // Upewnij się, że Feature ma ICollection<ResourceFeature> ResourceFeatures
                .HasForeignKey(rf => rf.FeatureId);
        });

        // Konfiguracja dla Booking
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(e => e.Status)
                .HasConversion<string>() // Przechowuj enum jako string
                .HasMaxLength(50); // Maksymalna długość dla stringa enum

            // Relacja Booking * --- 1 User (Użytkownik może mieć wiele rezerwacji)
            entity.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Nie usuwaj użytkownika, jeśli ma rezerwacje

            // Relacja Booking * --- 1 Resource (Zasób może mieć wiele rezerwacji)
            entity.HasOne(b => b.Resource)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.ResourceId)
                .OnDelete(DeleteBehavior.Cascade); // Usunięcie zasobu usuwa jego rezerwacje (do dyskusji)
        });

        // Konfiguracja dla ResourceType
        modelBuilder.Entity<ResourceType>(entity =>
        {
            entity.Property(rt => rt.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(rt => rt.Name).IsUnique(); // Nazwa typu zasobu musi być unikalna
        });

        // Konfiguracja dla Feature
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.Property(f => f.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(f => f.Name).IsUnique(); // Nazwa cechy musi być unikalna
        });

        // Konfiguracja dla User (w ramach Identity)
        // IdentityDbContext sam konfiguruje większość dla User, ale można dostosować
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(50);
            entity.Property(u => u.LastName).HasMaxLength(50);
            // Dodatkowe pola z planu dla User można tu konfigurować, np. Department, JobTitle
        });
    }
}