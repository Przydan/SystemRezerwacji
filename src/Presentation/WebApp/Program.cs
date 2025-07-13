using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Auth;
using WebApp.Services;
using IResourceService = WebApp.Services.IResourceService;

namespace WebApp
{
    public static class Program // Możesz dodać deklarację klasy, jeśli jej nie ma
    {
        public static async Task Main(string[] args)
        {
            // ======================= NAJWAŻNIEJSZY LOG TERAZ =======================
            Console.WriteLine(
                $"WebApp Program.Main: STARTING CLIENT-SIDE. Timestamp: {DateTime.Now.ToLongTimeString()}");
            // =======================================================================

            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            Console.WriteLine("WebApp Program.Main: HostBuilder created.");

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            Console.WriteLine("WebApp Program.Main: Root components added.");

            builder.Services.AddMudServices();
            Console.WriteLine("WebApp Program.Main: MudServices added.");

            builder.Services.AddBlazoredLocalStorage();
            Console.WriteLine("WebApp Program.Main: BlazoredLocalStorage added.");

            builder.Services.AddAuthorizationCore();
            Console.WriteLine("WebApp Program.Main: AuthorizationCore added.");

            builder.Services.AddScoped<AuthTokenHandler>();
            Console.WriteLine("WebApp Program.Main: AuthTokenHandler scoped.");

            builder.Services.AddHttpClient("SystemRezerwacji.API", client =>
                {
                    Console.WriteLine(
                        $"WebApp Program.Main: Configuring HttpClient SystemRezerwacji.API with base address: {builder.Configuration["ApiBaseUrl"]}");
                    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]
                                                 ?? throw new InvalidOperationException("ApiBaseUrl not configured"));
                })
                .AddHttpMessageHandler<AuthTokenHandler>();
            Console.WriteLine("WebApp Program.Main: HttpClient SystemRezerwacji.API configured.");

            builder.Services.AddScoped(sp =>
            {
                Console.WriteLine("WebApp Program.Main: Creating HttpClient from factory for SystemRezerwacji.API.");
                return sp.GetRequiredService<IHttpClientFactory>().CreateClient("SystemRezerwacji.API");
            });
            Console.WriteLine("WebApp Program.Main: Scoped HttpClient from factory registered.");

            // Upewnij się, że AuthService używa UPROSZCZONEJ GetAuthenticationStateAsync
            builder.Services.AddScoped<AuthService>();
            Console.WriteLine("WebApp Program.Main: AuthService scoped.");

            builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
            {
                Console.WriteLine("WebApp Program.Main: Resolving AuthService for AuthenticationStateProvider.");
                return provider.GetRequiredService<AuthService>();
            });
            Console.WriteLine("WebApp Program.Main: AuthenticationStateProvider registered.");

            builder.Services.AddScoped<IAuthService>(provider =>
            {
                Console.WriteLine("WebApp Program.Main: Resolving AuthService for IAuthService.");
                return provider.GetRequiredService<AuthService>();
            });
            Console.WriteLine("WebApp Program.Main: IAuthService registered.");

            builder.Services.AddScoped<IResourceService, ResourceService>();
            builder.Services.AddScoped<IResourceTypeService, ResourceTypeService>();
            // builder.Services.AddScoped<IBookingService, BookingService>();
            Console.WriteLine("WebApp Program.Main: Application services registered.");

            Console.WriteLine("WebApp Program.Main: Building host...");
            var host = builder.Build();
            Console.WriteLine("WebApp Program.Main: Host built. Running async...");
            await host.RunAsync();
            Console.WriteLine(
                "WebApp Program.Main: RunAsync completed (this log might not appear if RunAsync blocks indefinitely).");
        }
    }
}