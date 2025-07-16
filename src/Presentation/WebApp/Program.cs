using Application.Interfaces.Booking;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Auth;
using WebApp.Services;

using Application.Interfaces.Persistence;
using Application.Interfaces.User;

namespace WebApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddMudServices();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthTokenHandler>();

            builder.Services.AddHttpClient("SystemRezerwacji.API", client =>
                {
                    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]
                                                 ?? throw new InvalidOperationException("ApiBaseUrl not configured"));
                })
                .AddHttpMessageHandler<AuthTokenHandler>();

            builder.Services.AddScoped(sp => 
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("SystemRezerwacji.API"));
            
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AuthService>());
            builder.Services.AddScoped<IAuthService>(provider => provider.GetRequiredService<AuthService>());
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IResourceService, ResourceService>();
            builder.Services.AddScoped<IResourceTypeService, ResourceTypeService>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            await builder.Build().RunAsync();
        }
    }
}