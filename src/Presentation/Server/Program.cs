using System.Text;
using Application.Interfaces.Booking;
using Application.Interfaces.Identity;
using Application.Interfaces.Persistence;
using Application.Interfaces.User;
using Application.Services;
using Domain.Entities;
using Infrastructure.Authentication;
using Infrastructure.Identity;
using Infrastructure.Persistence.DbContext;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seed;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        ConfigureCorsPolicy(builder);
        ConfigureApplicationServices(builder);
    }

    private static void ConfigureCorsPolicy(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorApp", policyBuilder =>
                policyBuilder
                    .WithOrigins(builder.Configuration["WebAppBaseUrl"] ?? "http://localhost:5214")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });
    }

    private static void ConfigureBasicServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
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
            options.UseSqlite(connectionString));

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
        var jwtSettingsSection = builder.Configuration.GetSection(JwtSettings.SectionName);
        var jwtSettings = jwtSettingsSection.Get<JwtSettings>();

        if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
        {
            throw new InvalidOperationException("Konfiguracja JWT (JwtSettings) jest nieprawidłowa lub klucz (Key) jest pusty.");
        }
        
        builder.Services.Configure<JwtSettings>(jwtSettingsSection);
        
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ClockSkew = TimeSpan.FromSeconds(5)
            };
        });
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministratorRole", policy =>
                policy.RequireRole("Administrator"));
        });
    }

    private static void ConfigureApplicationServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUserService, UserService>();
        
        builder.Services.AddScoped<IResourceTypeRepository, ResourceTypeRepository>();
        builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
        
        builder.Services.AddScoped<IResourceTypeService, ResourceTypeService>();
        builder.Services.AddScoped<IResourceService, ResourceService>();
        
        builder.Services.AddScoped<IBookingService, BookingService>();

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
        app.UseCors("AllowBlazorApp");
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

            // Wywołaj wszystkie seedery, przekazując dostawcę usług
            await IdentityDataSeeder.SeedRolesAndAdminUserAsync(services);
            //await ResourceTypeSeeder.SeedResourceTypeAsync(services);
            //await ResourceSeeder.SeedAsync(services); 
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Wystąpił błąd podczas seedowania bazy danych.");
        }
    }
}