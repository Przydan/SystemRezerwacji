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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ResourceType>()
            .HasIndex(rt => rt.Name).IsUnique();
    }
}