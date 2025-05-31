using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.DTOs.Auth;
using WebApp.Auth;

namespace WebApp.Services
{
    public class AuthService(HttpClient http, ILocalStorageService storage) : AuthenticationStateProvider, IAuthService
    {
        // Metoda LoginAsync - pozostaje bez zmian
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var response = await http.PostAsJsonAsync("api/auth/login", dto);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new AuthResponseDto
                    { IsSuccess = false, Message = $"Błąd logowania: {response.StatusCode}. {errorContent}" };
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (authResponse is { IsSuccess: true } && !string.IsNullOrEmpty(authResponse.Token))
            {
                await storage.SetItemAsync("authToken", authResponse.Token);
                NotifyAuthenticationStateChanged(
                    GetAuthenticationStateAsync()); // Wywołaj oryginalną metodę, ale teraz będzie uproszczona
                return authResponse;
            }
            else
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Logowanie nie powiodło się. Brak tokena lub odpowiedź API wskazuje na błąd."
                };
            }
        }
        
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            var response = await http.PostAsJsonAsync("api/auth/register", registerRequest);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new AuthResponseDto
                    { IsSuccess = false, Message = $"Błąd rejestracji: {response.StatusCode}. {errorContent}" };
            }

            var registrationResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return registrationResponse ?? new AuthResponseDto
                { IsSuccess = false, Message = "Nie udało się przetworzyć odpowiedzi serwera po rejestracji." };
        }
        
        public async Task LogoutAsync()
        {
            await storage.RemoveItemAsync("authToken");
            NotifyAuthenticationStateChanged(
                GetAuthenticationStateAsync()); // Wywołaj oryginalną metodę, ale teraz będzie uproszczona
        }
        
        
         public override async Task<AuthenticationState> GetAuthenticationStateAsync()
         {
             Console.WriteLine("AuthService.GetAuthenticationStateAsync: Rozpoczynam pobieranie stanu autentykacji (oryginalna metoda).");
             var savedToken = await storage.GetItemAsStringAsync("authToken");
             // Dodaj logowanie wartości tokenu (ale ostrożnie, nie loguj samego tokenu w produkcji)
             Console.WriteLine($"AuthService.GetAuthenticationStateAsync: Token z localStorage: {(string.IsNullOrEmpty(savedToken) ? "BRAK" : "OBECNY")}");

             var identity = new ClaimsIdentity();
             http.DefaultRequestHeaders.Authorization = null; // Zresetuj nagłówek na wszelki wypadek

             if (!string.IsNullOrEmpty(savedToken))
             {
                 try
                 {
                     identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(savedToken), "jwt");
                     http.DefaultRequestHeaders.Authorization =
                         new AuthenticationHeaderValue("Bearer", savedToken);
                     Console.WriteLine("AuthService.GetAuthenticationStateAsync: Tożsamość utworzona z tokenu, nagłówek Authorization ustawiony.");
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine($"AuthService.GetAuthenticationStateAsync: Błąd podczas parsowania tokenu lub ustawiania nagłówka: {ex.Message}");
                     await storage.RemoveItemAsync("authToken"); 
                     identity = new ClaimsIdentity();
                 }
             }
             else
             {
                  Console.WriteLine("AuthService.GetAuthenticationStateAsync: Brak zapisanego tokenu, tożsamość pozostaje pusta.");
             }
             return new AuthenticationState(new ClaimsPrincipal(identity));
         }
}
}