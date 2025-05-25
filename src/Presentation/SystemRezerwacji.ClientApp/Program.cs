using BLECoder.Blazor.Client.Authentication.Security;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SystemRezerwacji.ClientApp;
using MudBlazor.Services;
using SystemRezerwacji.ClientApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IAuthClientService, AuthClientService>();

builder.Services.AddHttpClient<BookingService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] 
                                 ?? throw new InvalidOperationException("ApiBaseUrl not configured in appsettings.json"));
});

builder.Services.AddHttpClient("SystemRezerwacji.ServerAPI", client => {
        client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] 
                                     ?? throw new InvalidOperationException("ApiBaseUrl not configured in appsettings.json"));
    })
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>(); // Automatycznie dołącza token

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("SystemRezerwacji.ServerAPI"));

builder.Services.AddScoped<ResourceClientService>(); // Zobacz modyfikację serwisu poniżej

builder.Services.AddApiAuthorization()
    .AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>(); // Dla ról jako tablica stringów


await builder.Build().RunAsync();
