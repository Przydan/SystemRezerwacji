using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.DTOs.Auth;


namespace WebApp.Services
{
    public class AuthService : AuthenticationStateProvider, IAuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _storage;

        public AuthService(HttpClient http, ILocalStorageService storage)
        {
            _http = http;
            _storage = storage;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", dto);
            if (!response.IsSuccessStatusCode)
            {
                // Możesz spróbować odczytać treść błędu z API, jeśli jest dostępna
                var errorContent = await response.Content.ReadAsStringAsync();
                // Zwróć obiekt AuthResponseDto z informacją o błędzie
                return new AuthResponseDto { IsSuccess = false, Message = $"Błąd logowania: {response.StatusCode}. {errorContent}" };
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (authResponse != null && authResponse.IsSuccess && !string.IsNullOrEmpty(authResponse.Token))
            {
                await _storage.SetItemAsync("authToken", authResponse.Token);
                // Poniższa linia jest poprawna, jeśli AuthService dziedziczy z AuthenticationStateProvider.
                // Jeśli to oddzielny IAuthService, NotifyAuthenticationStateChanged musiałoby być wywołane inaczej.
                // W Program.cs widzę: builder.Services.AddScoped<AuthenticationStateProvider, AuthService>();
                // builder.Services.AddScoped<IAuthService, AuthService>();
                // To oznacza, że ta sama instancja AuthService jest używana jako AuthenticationStateProvider i IAuthService.
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return authResponse;
            }
            else
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Logowanie nie powiodło się. Brak tokena lub odpowiedź API wskazuje na błąd." };
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", registerRequest);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new AuthResponseDto { IsSuccess = false, Message = $"Błąd rejestracji: {response.StatusCode}. {errorContent}" };
            }
            // Serwer dla /register zwraca AuthResponseDto z IsSuccess=true i Message, ale bez tokena.
            // Użytkownik musi się zalogować osobno.
            var registrationResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return registrationResponse ?? new AuthResponseDto { IsSuccess = false, Message = "Nie udało się przetworzyć odpowiedzi serwera po rejestracji." };
        }

        Task<AuthResponseDto> IAuthService.LoginAsync(LoginRequestDto loginRequest)
        {
            throw new NotImplementedException();
        }

        public async Task LogoutAsync()
        {
            await _storage.RemoveItemAsync("authToken");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _storage.GetItemAsStringAsync("authToken");
            var identity = new ClaimsIdentity();
            if (!string.IsNullOrEmpty(savedToken))
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt");
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", savedToken);
            }
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var json = Base64UrlDecode(payload);
            var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
            return claims.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
        }

        private static string Base64UrlDecode(string str)
        {
            str = str.Replace('-', '+').Replace('_', '/');
            switch (str.Length % 4)
            {
                case 2: str += "=="; break;
                case 3: str += "="; break;
            }
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
    }
}