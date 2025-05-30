using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SystemRezerwacji.WebApp;
using SystemRezerwacji.WebApp.Auth;
using SystemRezerwacji.WebApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. MudBlazor
builder.Services.AddMudServices();

// 2. Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// 3. Authorization Core
builder.Services.AddAuthorizationCore();

// 4. HTTP z tokenem
builder.Services.AddScoped<AuthTokenHandler>();
builder.Services.AddHttpClient("SystemRezerwacji.API", client =>
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]
        ?? throw new InvalidOperationException("ApiBaseUrl not configured")))
    .AddHttpMessageHandler<AuthTokenHandler>();
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("SystemRezerwacji.API"));

// 5. Custom AuthenticationStateProvider + IAuthService
builder.Services.AddScoped<AuthenticationStateProvider, AuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 6. Pozostałe serwisy
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IResourceTypeService, ResourceTypeService>();
builder.Services.AddScoped<IBookingService, BookingService>();

await builder.Build().RunAsync();