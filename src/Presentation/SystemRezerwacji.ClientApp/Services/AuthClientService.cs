using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using SystemRezerwacji.Application.DTOs.Auth;
using SystemRezerwacji.ClientApp.Models;

namespace SystemRezerwacji.ClientApp.Services;

 public interface IAuthClientService
    {
        Task<AuthResponseDto?> LoginAsync(LoginRequest loginRequest);
        Task<AuthResponseDto?> RegisterAsync(RegisterUserDto registerUserDto);
        Task LogoutAsync(); // Wylogowanie jest obsługiwane przez mechanizmy Blazor
    }

    public class AuthClientService : IAuthClientService
    {
        private readonly HttpClient _httpClient;
        // NavigationManager jest potrzebny do przekierowania po wylogowaniu
        private readonly NavigationManager _navigationManager;


        public AuthClientService(IHttpClientFactory clientFactory, NavigationManager navigationManager)
        {
            _httpClient = clientFactory.CreateClient("SystemRezerwacji.ServerAPI"); // Użyj nazwanego klienta
            _navigationManager = navigationManager;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequest loginRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }
            // Możesz chcieć odczytać treść błędu, jeśli API zwraca szczegóły
            // np. var errorContent = await response.Content.ReadAsStringAsync();
            // i przekazać go dalej lub zalogować
            return new AuthResponseDto { IsSuccess = false, Message = $"Błąd logowania: {response.ReasonPhrase}" };
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterUserDto registerUserDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerUserDto);
             if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }
            // Podobnie, obsługa błędów
            var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return errorResponse ?? new AuthResponseDto { IsSuccess = false, Message = $"Błąd rejestracji: {response.ReasonPhrase}" };
        }
        
        public async Task LogoutAsync()
        {
            // Mechanizm wylogowania w Blazor WASM z `AddApiAuthorization`
            // zazwyczaj polega na przekierowaniu do endpointu wylogowania Identity.
            // Jeśli używasz `AddApiAuthorization()`, to ono obsługuje wylogowanie.
            // To jest przykład, jak można by to zainicjować.
            // `SignOutSessionStateManager` może być używany do zarządzania stanem wylogowania.
            // Dla standardowej konfiguracji z `AddApiAuthorization`, przekierowanie jest kluczowe.
            _navigationManager.NavigateToLogout("authentication/logout");
            await Task.CompletedTask; // Logout jest bardziej przekierowaniem niż wywołaniem API
        }
    }