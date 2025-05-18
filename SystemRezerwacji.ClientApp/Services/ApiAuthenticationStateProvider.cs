namespace SystemRezerwacji.ClientApp.Services
{
    // SystemRezerwacji.ClientApp/Services/ApiAuthenticationStateProvider.cs
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.JSInterop;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient; // Potrzebny do ustawiania domyślnego nagłówka autoryzacji
        private const string TokenKey = "authToken";

        public ApiAuthenticationStateProvider(IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _jsRuntime = jsRuntime;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
            var identity = new ClaimsIdentity();
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadJwtToken(token);

                    // Sprawdzenie, czy token nie wygasł
                    if (jwtToken.ValidTo > DateTime.UtcNow)
                    {
                        identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                        _httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
                    }
                    else
                    {
                        // Token wygasł, usuń go
                        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Błąd podczas przetwarzania tokenu JWT: " + ex.ToString());
                    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey); // Usuń nieprawidłowy token
                }
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void MarkUserAsAuthenticated(string token)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void MarkUserAsLoggedOut()
        {
            var identity = new ClaimsIdentity();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var payload = token.Claims; // Bezpośredni dostęp do claims z JwtSecurityToken

            claims.AddRange(payload); // Dodaj wszystkie claims z tokenu

            // Możesz chcieć specyficznie zmapować niektóre claims, jeśli backend ich tak nie nazywa
            // Na przykład, jeśli backend używa 'unique_name' dla nazwy użytkownika:
            // var usernameClaim = payload.FirstOrDefault(c => c.Type == "unique_name");
            // if (usernameClaim != null)
            // {
            //     claims.Add(new Claim(ClaimTypes.Name, usernameClaim.Value));
            // }

            return claims;
        }
    }
}

