using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SystemRezerwacji.WebApp.Auth;
using SystemRezerwacji.WebApp.Models;

namespace SystemRezerwacji.WebApp.Services
{
    public class AuthService : AuthenticationStateProvider
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _storage;

        public AuthService(HttpClient http, ILocalStorageService storage)
        {
            _http = http;
            _storage = storage;
        }

        public async Task<bool> LoginAsync(LoginRequestDto dto)
        {
            var resp = await _http.PostAsJsonAsync("api/auth/login", dto);
            if (!resp.IsSuccessStatusCode) return false;
            var jwt = await resp.Content.ReadAsStringAsync();
            await _storage.SetItemAsync("authToken", jwt);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return true;
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