using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization; // Błąd CS0234 nadal może tu być, jeśli paczka NuGet jest problemem
using MudBlazor.Services;
using SystemRezerwacji.ClientApp; // Jeśli ApiAuthenticationStateProvider jest tutaj
using SystemRezerwacji.ClientApp.Services; // Jeśli ApiAuthenticationStateProvider jest tutaj

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices(); // Zakładając, że MudBlazor jest zainstalowany

// Rejestracja serwisów autoryzacji
builder.Services.AddScoped<ApiAuthenticationStateProvider>(); // Błąd CS0246, jeśli klasa nie jest znaleziona
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<ApiAuthenticationStateProvider>());
builder.Services.AddScoped<IAuthService, AuthService>(); // Upewnij się, że AuthService i IAuthService są zdefiniowane i w poprawnych namespace
builder.Services.AddAuthorizationCore();

// Rejestracja serwisu zasobów
builder.Services.AddScoped<IResourceService, ResourceService>(); // Upewnij się, że ResourceService i IResourceService są zdefiniowane

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/") });

await builder.Build().RunAsync();
// ŻADNEJ DODATKOWEJ KLAMRY TUTAJ