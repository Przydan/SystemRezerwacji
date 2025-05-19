// SystemRezerwacji.ClientApp/Services/AuthService.cs
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop; // Potrzebne dla localStorage
using SystemRezerwacji.ClientApp.Models;
using System.Text.Json;
using SystemRezerwacji.ClientApp.Services;


public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime; // Do interakcji z localStorage
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    private const string TokenKey = "authToken"; // Klucz do przechowywania tokenu w localStorage

    // Przykładowy model odpowiedzi z API po logowaniu, który zawiera token
    private class LoginResponse
    {
        public string? Token { get; set; }
        // Możesz dodać inne dane zwracane przez API, np. dane użytkownika
    }

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<string?> GetToken()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
    }

    public async Task<bool> Login(LoginRequest model)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", model);

        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (!string.IsNullOrEmpty(loginResponse?.Token))
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, loginResponse.Token);
                // Powiadomienie o zmianie stanu uwierzytelnienia
                ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResponse.Token);
                return true;
            }
        }
        // Tutaj można dodać obsługę błędów, np. odczytanie wiadomości błędu z response.Content
        Console.WriteLine($"Błąd logowania: {await response.Content.ReadAsStringAsync()}");
        return false;
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        // Powiadomienie o zmianie stanu uwierzytelnienia
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
    }

    public async Task<bool> Register(RegisterRequest model)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", model);

        if (response.IsSuccessStatusCode)
        {
            // Opcjonalnie: automatyczne logowanie po rejestracji
            // var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            // if (!string.IsNullOrEmpty(loginResponse?.Token))
            // {
            //     await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, loginResponse.Token);
            //     ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResponse.Token);
            // }
            return true;
        }
        // Tutaj można dodać obsługę błędów
        Console.WriteLine($"Błąd rejestracji: {await response.Content.ReadAsStringAsync()}");
        return false;
    }
}
