using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using SystemRezerwacji.Infrastructure.Persistence.DbContext; // Ścieżka do Twojego DbContext
using SystemRezerwacji.Domain.Entities; // Dla User
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SystemRezerwacji.Application.Features.ResourceType.Queries.GetAllResourceTypes;
using SystemRezerwacji.Application.Interfaces.Identity;
using SystemRezerwacji.Application.Interfaces.Persistence;
using SystemRezerwacji.Application.Services;
using SystemRezerwacji.Infrastructure.Persistence.Repositories;
using SystemRezerwacji.Infrastructure.Persistence.Seed; // Dla IdentityRole i AddIdentity


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// --- POCZĄTEK KONFIGURACJI BAZY DANYCH I IDENTITY ---

// 1. Konfiguracja Connection String dla SQLite
// Odczyt z appsettings.json LUB domyślna wartość, jeśli nie znaleziono
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=systemrezerwacji.db"; // Domyślna nazwa pliku bazy SQLite

// 2. Rejestracja DbContext z dostawcą SQLite
builder.Services.AddDbContext<SystemRezerwacjiDbContext>(options =>
    options.UseSqlite(connectionString)); // Użyj UseSqlite

// 3. Rejestracja ASP.NET Core Identity
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        // Tutaj można skonfigurować opcje Identity (polityki haseł, blokady itp.)
        // zgodnie z planem projektowym (sekcja 4.1)
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8; // Możesz zmniejszyć na potrzeby deweloperskie, jeśli chcesz
        options.Password.RequireNonAlphanumeric = false; // Uproszczenie dla dewelopmentu
        options.Password.RequireUppercase = false; // Uproszczenie dla dewelopmentu
        options.Password.RequireLowercase = false; // Uproszczenie dla dewelopmentu

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false; // Zmień na true jeśli potrzebujesz potwierdzenia email
    })
    .AddEntityFrameworkStores<SystemRezerwacjiDbContext>() // Wskazanie EF Core jako magazynu
    .AddDefaultTokenProviders(); // Dla generowania tokenów (np. reset hasła)

// --- KONIEC KONFIGURACJI BAZY DANYCH I IDENTITY ---

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true; // Opcjonalnie, jeśli token ma być dostępny w HttpContext
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment(); // W dev może być false, w prod true
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]
                                                                           ?? throw new InvalidOperationException("JWT Key not configured in JwtSettings:Key."))),
        ClockSkew = TimeSpan.Zero // Brak tolerancji dla wygaśnięcia tokenu (zalecane)
    };
});

// Dodanie polityk autoryzacji (na razie jedna przykładowa)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
    // Tutaj można dodawać inne polityki w przyszłości, np. "RequireUserRole"
});

// Rejestracja IAuthService (dodasz ją w kolejnym podpunkcie)
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddScoped<IResourceTypeRepository, ResourceTypeRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllResourceTypesQuery).Assembly));
builder.Services.AddScoped<IResourceTypeService, ResourceTypeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Seedowanie danych przy starcie aplikacji (tylko w Development)
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SystemRezerwacjiDbContext>();
        // Upewnij się, że baza danych jest utworzona i migracje są zastosowane
        await context.Database.MigrateAsync();

        // Wywołanie seedera dla ról i użytkownika admina
        await IdentityDataSeeder.SeedRolesAndAdminUserAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();