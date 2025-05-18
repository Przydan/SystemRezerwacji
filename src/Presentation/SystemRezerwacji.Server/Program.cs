using Microsoft.EntityFrameworkCore;
using SystemRezerwacji.Infrastructure.Persistence.DbContext; // Ścieżka do Twojego DbContext
using SystemRezerwacji.Domain.Entities; // Dla User
using Microsoft.AspNetCore.Identity; // Dla IdentityRole i AddIdentity


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();