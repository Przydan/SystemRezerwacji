using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SystemRezerwacji.Application.Features.ResourceType.Queries.GetAllResourceTypes;
using SystemRezerwacji.Application.Interfaces.Identity;
using SystemRezerwacji.Application.Interfaces.Persistence;
using SystemRezerwacji.Application.Services;
using SystemRezerwacji.Domain.Entities;
using SystemRezerwacji.Infrastructure.Persistence.DbContext;
using SystemRezerwacji.Infrastructure.Persistence.Repositories;
using SystemRezerwacji.Infrastructure.Persistence.Seed;

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
        // Podstawowe usługi
        ConfigureBasicServices(builder);
        
        // Baza danych i Identity
        ConfigureDatabaseAndIdentity(builder);
        
        // Autentykacja i Autoryzacja
        ConfigureAuthenticationAndAuthorization(builder);
        
        // Serwisy aplikacyjne
        ConfigureApplicationServices(builder);
    }

    private static void ConfigureBasicServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    private static void ConfigureDatabaseAndIdentity(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? $"Data Source={DefaultDbName}";

        builder.Services.AddDbContext<SystemRezerwacjiDbContext>(options =>
            options.UseSqlite(connectionString));

        builder.Services.AddIdentity<User, IdentityRole<Guid>>(ConfigureIdentityOptions)
            .AddEntityFrameworkStores<SystemRezerwacjiDbContext>()
            .AddDefaultTokenProviders();
    }

    private static void ConfigureIdentityOptions(IdentityOptions options)
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    }

    private static void ConfigureAuthenticationAndAuthorization(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => ConfigureJwtOptions(options, builder));

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministratorRole", policy => 
                policy.RequireRole("Administrator"));
        });
    }

    private static void ConfigureJwtOptions(JwtBearerOptions options, WebApplicationBuilder builder)
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"] 
                    ?? throw new InvalidOperationException("JWT Key not configured in JwtSettings:Key."))),
            ClockSkew = TimeSpan.Zero
        };
    }

    private static void ConfigureApplicationServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IResourceTypeRepository, ResourceTypeRepository>();
        builder.Services.AddScoped<IResourceTypeService, ResourceTypeService>();
        builder.Services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(GetAllResourceTypesQuery).Assembly));
    }

    private static async Task ConfigureApplication(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            await SeedDevelopmentData(app);
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
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
            await IdentityDataSeeder.SeedRolesAndAdminUserAsync(services);
            await ResourceTypeSeeder.SeedResourceTypeAsync(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}