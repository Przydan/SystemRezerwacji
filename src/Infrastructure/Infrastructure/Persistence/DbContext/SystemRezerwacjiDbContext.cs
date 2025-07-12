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
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(d => d.ResourceType)
                .WithMany(p => p.Resources)
                .HasForeignKey(d => d.ResourceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Konfiguracja dla ResourceFeature
        modelBuilder.Entity<ResourceFeature>(entity =>
        {
            entity.HasKey(rf => new { rf.ResourceId, rf.FeatureId });
            
            entity.HasOne(rf => rf.Resource)
                .WithMany(r =>
                    r.ResourceFeatures)
                .HasForeignKey(rf => rf.ResourceId);
            
            entity.HasOne(rf => rf.Feature)
                .WithMany(f =>
                    f.ResourceFeatures)
                .HasForeignKey(rf => rf.FeatureId);
        });

        // Konfiguracja dla Booking
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50);
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();

            // Relacja Booking * --- 1 User (Użytkownik może mieć wiele rezerwacji)
            entity.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacja Booking * --- 1 Resource (Zasób może mieć wiele rezerwacji)
            entity.HasOne(b => b.Resource)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Konfiguracja dla ResourceType
        modelBuilder.Entity<ResourceType>(entity =>
        {
            entity.Property(rt => rt.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(rt => rt.Name).IsUnique();
        });

        // Konfiguracja dla Feature
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.Property(f => f.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(f => f.Name).IsUnique();
        });

        // Konfiguracja dla User (w ramach Identity)
        // IdentityDbContext sam konfiguruje większość dla User, ale można dostosować
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(50);
            entity.Property(u => u.LastName).HasMaxLength(50);
        });
    }
}