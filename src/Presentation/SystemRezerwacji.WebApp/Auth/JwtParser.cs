using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace SystemRezerwacji.WebApp.Auth;

public static class JwtParser
{
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1]; // Bierzemy tylko payload

        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            // Wyciągamy role (mogą być jako pojedynczy string lub tablica)
            if (keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles))
            {
                if (roles is JsonElement rolesElement)
                {
                    if (rolesElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var role in rolesElement.EnumerateArray())
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.GetString() ?? ""));
                        }
                    }
                    else if (rolesElement.ValueKind == JsonValueKind.String)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rolesElement.GetString() ?? ""));
                    }
                }
                else if (roles is string rolesString) // Na wszelki wypadek
                {
                     claims.Add(new Claim(ClaimTypes.Role, rolesString));
                }
            }
            keyValuePairs.Remove(ClaimTypes.Role); // Usuwamy, aby nie dodać ponownie

            // Dodajemy pozostałe claims
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? "")));
        }
        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}