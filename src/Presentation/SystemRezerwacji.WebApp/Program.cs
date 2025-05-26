using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SystemRezerwacji.WebApp;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SystemRezerwacji.WebApp.Auth;
using SystemRezerwacji.WebApp.Services;
using SystemRezerwacji.WebApp.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// --- Konfiguracja Usług ---

// 1. MudBlazor
builder.Services.AddMudServices();

// 2. Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// 3. Autoryzacja Core
builder.Services.AddAuthorizationCore();

// 4. Delegating Handler (WAŻNE: zarejestruj go)
builder.Services.AddScoped<AuthTokenHandler>();

// 5. HttpClient Factory z Handlerem
builder.Services.AddHttpClient("SystemRezerwacji.API", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]
            ?? throw new InvalidOperationException("ApiBaseUrl not configured"));
    })
    .AddHttpMessageHandler<AuthTokenHandler>(); // <-- Dodajemy nasz handler

// 6. Udostępnienie HttpClient dla AuthProvider (i innych, jeśli potrzebują)
//    Używamy "czystego" HttpClienta, bo handler doda token.
//    Można też wstrzykiwać IHttpClientFactory i tworzyć klienta tam, gdzie potrzebny.
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("SystemRezerwacji.API"));

// 7. Custom AuthenticationStateProvider
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

// 8. Rejestracja serwisów aplikacyjnych
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IResourceTypeService, ResourceTypeService>();

// Tutaj dodasz inne serwisy, np. ResourceTypeService

// --- Budowanie Aplikacji ---
await builder.Build().RunAsync();