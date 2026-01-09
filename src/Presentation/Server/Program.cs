using System.Text;
using Application.Interfaces.Booking;
using Application.Interfaces.Persistence;
using Application.Interfaces.User;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence.DbContext;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seed;
using Infrastructure.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.OpenApi.Models;

namespace Server;

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
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddAutoMapper(typeof(Application.Mappings.MappingProfile).Assembly);
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "SystemRezerwacji API", Version = "v1" });
            
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Wprowadź token JWT poprzedzony słowem 'Bearer ' (np. 'Bearer eyJhbGci...')",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
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
            // Opcjonalne: dostosowanie ustawień ciasteczka
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        });
    }

    private static void ConfigureApplicationServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserService, UserService>();
        
        builder.Services.AddScoped<IResourceTypeRepository, ResourceTypeRepository>();
        builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
        
       // Services
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IResourceTypeService, ResourceTypeService>();
builder.Services.AddScoped<Application.Interfaces.Infrastructure.IEmailService, Infrastructure.Services.FileEmailService>();
    }

    private static async Task ConfigureApplication(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            await SeedDevelopmentData(app);
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

        try
        {
            var context = services.GetRequiredService<SystemRezerwacjiDbContext>();
            await context.Database.MigrateAsync();

            // Wywołaj wszystkie seedery, przekazując dostawcę usług
            await IdentityDataSeeder.SeedRolesAndAdminUserAsync(services);
            await ResourceTypeSeeder.SeedResourceTypeAsync(services);
            await Infrastructure.Persistence.Seed.ResourceSeeder.SeedResourcesAsync(context);
        
            // Seed Bookings
            var logger = services.GetRequiredService<ILogger<Program>>();
            await Infrastructure.Persistence.Seed.BookingSeeder.SeedBookingsAsync(context, logger);
        
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Wystąpił błąd podczas seedowania bazy danych.");
        }
    }
}