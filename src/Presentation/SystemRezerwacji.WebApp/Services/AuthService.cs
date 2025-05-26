using SystemRezerwacji.WebApp.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using SystemRezerwacji.WebApp.Auth; // Potrzebne do CustomAuthenticationStateProvider

namespace SystemRezerwacji.WebApp.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthenticationStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        // Musimy rzutować, aby mieć dostęp do MarkUserAs...
        _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        if (authResponse != null && authResponse.IsSuccess && !string.IsNullOrEmpty(authResponse.Token))
        {
            await _authStateProvider.MarkUserAsAuthenticated(authResponse.Token);
            return authResponse;
        }

        return authResponse ?? new AuthResponseDto { IsSuccess = false, Message = "Wystąpił nieznany błąd logowania." };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerRequest);
        return await response.Content.ReadFromJsonAsync<AuthResponseDto>()
               ?? new AuthResponseDto { IsSuccess = false, Message = "Wystąpił nieznany błąd rejestracji." };
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.MarkUserAsLoggedOut();
    }
}