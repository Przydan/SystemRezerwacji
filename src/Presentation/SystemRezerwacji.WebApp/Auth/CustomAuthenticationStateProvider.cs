using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SystemRezerwacji.WebApp.Auth;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient; // Potrzebny do ustawiania domyślnego nagłówka

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    private const string TokenKey = "authToken";

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await _localStorage.GetItemAsync<string>(TokenKey);

        if (string.IsNullOrWhiteSpace(savedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            var claims = JwtParser.ParseClaimsFromJwt(savedToken);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            // Sprawdzenie wygaśnięcia tokenu (proste)
            var expiry = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
            if (expiry != null)
            {
                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiry));
                if (expiryDate.UtcDateTime <= DateTime.UtcNow)
                {
                    await MarkUserAsLoggedOut(); // Wygasł, wyloguj
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
            return new AuthenticationState(user);
        }
        catch
        {
            // Jeśli token jest nieprawidłowy, wyloguj
            await MarkUserAsLoggedOut();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await _localStorage.SetItemAsync(TokenKey, token);
        var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;

        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}