using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence.DbContext;

public class SystemRezerwacjiDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public SystemRezerwacjiDbContext(DbContextOptions<SystemRezerwacjiDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Resource> Resources { get; set; }
    public DbSet<ResourceType> ResourceTypes { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<ResourceFeature> ResourceFeatures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // To jest WAŻNE dla Identity

        // --- Konfiguracja relacji wiele-do-wielu dla Resource <--> Feature ---

        // 1. Zdefiniuj klucz złożony dla tabeli łączącej ResourceFeature
        modelBuilder.Entity<ResourceFeature>()
            .HasKey(rf => new { rf.ResourceId, rf.FeatureId });

        // 2. Zdefiniuj relację od strony Resource
        modelBuilder.Entity<ResourceFeature>()
            .HasOne(rf => rf.Resource)
            .WithMany(r => r.ResourceFeatures) // <-- wskazuje na ICollection<ResourceFeature> w klasie Resource
            .HasForeignKey(rf => rf.ResourceId);

        // 3. Zdefiniuj relację od strony Feature
        modelBuilder.Entity<ResourceFeature>()
            .HasOne(rf => rf.Feature)
            .WithMany(f => f.ResourceFeatures) // <-- wskazuje na ICollection<ResourceFeature> w klasie Feature
            .HasForeignKey(rf => rf.FeatureId);

        // --- Pozostałe konfiguracje ---
        modelBuilder.Entity<ResourceType>()
            .HasIndex(rt => rt.Name).IsUnique();

        modelBuilder.Entity<Feature>()
            .HasIndex(f => f.Name).IsUnique();
    }
}