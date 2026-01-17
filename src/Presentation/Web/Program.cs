using Application.Interfaces.Booking;
using Application.Interfaces.Persistence;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence.DbContext;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seed;
using Infrastructure.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Web;

public class Program
{
    private const string DefaultDbName = "systemrezerwacji.db";
    

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);

        var app = builder.Build();
        await ConfigureApplication(app);
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        ConfigureBasicServices(builder);
        ConfigureDatabaseAndIdentity(builder);
        ConfigureAuthenticationAndAuthorization(builder);

        ConfigureApplicationServices(builder);
    }



    private static void ConfigureBasicServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();
        builder.Services.AddAutoMapper(typeof(Application.Mappings.MappingProfile).Assembly);
    }

    private static void ConfigureDatabaseAndIdentity(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? $"Data Source={DefaultDbName}";

        builder.Services.AddDbContext<SystemRezerwacjiDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddIdentity<User, IdentityRole<Guid>>(ConfigureIdentityOptions)
            .AddEntityFrameworkStores<SystemRezerwacjiDbContext>()
            .AddDefaultTokenProviders();
    }

    private static void ConfigureIdentityOptions(IdentityOptions options)
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 12;
        
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
        
        options.User.RequireUniqueEmail = true;
        
        options.SignIn.RequireConfirmedAccount = false;
    }

    private static void ConfigureAuthenticationAndAuthorization(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministratorRole", policy =>
                policy.RequireRole("Administrator"));
        });

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        });
    }

    private static void ConfigureApplicationServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IResourceTypeRepository, ResourceTypeRepository>();
        builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
        
        builder.Services.AddScoped<IResourceService, ResourceService>();
        builder.Services.AddScoped<IBookingService, BookingService>();
        builder.Services.AddScoped<IResourceTypeService, ResourceTypeService>();
        builder.Services.AddScoped<Application.Interfaces.Infrastructure.IEmailService, Infrastructure.Services.FileEmailService>();
    }

    private static async Task ConfigureApplication(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            await SeedDevelopmentData(app);
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        await app.RunAsync();
    }

    private static async Task SeedDevelopmentData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // Read SeedTestData flag, default to true if not set
        bool seedTestData = configuration.GetValue<bool>("SeedTestData", true);

        try
        {
            var context = services.GetRequiredService<SystemRezerwacjiDbContext>();
            
            // Retry logic for Docker environments where DB might not be fully ready
            const int maxRetries = 30;
            const int delaySeconds = 2;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    logger.LogInformation("Próba połączenia z bazą danych ({Attempt}/{MaxRetries})...", attempt, maxRetries);
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migracja bazy danych zakończona pomyślnie.");
                    break;
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    logger.LogWarning("Nie można połączyć się z bazą danych. Ponowna próba za {Delay}s... ({Message})", delaySeconds, ex.Message);
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }

            // Wywołaj wszystkie seedery, przekazując dostawcę usług
            // Identity (Roles & Admin) always runs, Test Users controlled by flag
            await IdentityDataSeeder.SeedRolesAndAdminUserAsync(services, seedTestUsers: seedTestData);
            
            // ResourceTypes are essential configuration, always run
            await ResourceTypeSeeder.SeedResourceTypeAsync(services);

            if (seedTestData)
            {
                // Resources and Bookings are test data
                await Infrastructure.Persistence.Seed.ResourceSeeder.SeedResourcesAsync(context);
            
                // Seed Bookings
                await Infrastructure.Persistence.Seed.BookingSeeder.SeedBookingsAsync(context, logger);
            }
        
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Wystąpił błąd podczas seedowania bazy danych.");
        }
    }
}