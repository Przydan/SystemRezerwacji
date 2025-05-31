using System.Security.Claims;
using System.Text.Json; // Upewnij się, że masz ten using
using System.Collections.Generic; // Dla List<T>
using System.Linq; // Dla Select
using System; // Dla Convert

namespace WebApp.Auth;

public static class JwtParser
{
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);
        // Używamy JsonSerializerOptions, aby nazwy właściwości były traktowane bez rozróżniania wielkości liter,
        // na wypadek gdyby klucze w JWT miały inną wielkość liter niż oczekujemy.
        // Jednak domyślnie JsonSerializer jest case-sensitive dla kluczy.
        // Bezpieczniej jest jawnie sprawdzać klucze, które znamy.
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            // --- POCZĄTEK ZMIAN ---
            string actualRoleClaimKeyInToken = "";

            if (keyValuePairs.ContainsKey("role"))
            {
                actualRoleClaimKeyInToken = "role";
            }
            else if (keyValuePairs.ContainsKey(ClaimTypes.Role)) // Sprawdź też pełny URI, na wszelki wypadek
            {
                actualRoleClaimKeyInToken = ClaimTypes.Role;
            }

            if (!string.IsNullOrEmpty(actualRoleClaimKeyInToken) &&
                keyValuePairs.TryGetValue(actualRoleClaimKeyInToken, out object? rolesValue))
            {
                if (rolesValue is JsonElement rolesElement)
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
                else if (rolesValue is string rolesString) // Obsługa, jeśli wartość jest bezpośrednio stringiem
                {
                    claims.Add(new Claim(ClaimTypes.Role, rolesString));
                }

                keyValuePairs.Remove(actualRoleClaimKeyInToken); // Usuń przetworzony claim roli
            }
            // --- KONIEC ZMIAN ---

            // Dodajemy pozostałe claims
            // Upewnij się, że klucze, których nie przetworzyłeś jako role, nie są ponownie dodawane, jeśli ich nazwy są mapowane
            // na standardowe ClaimTypes, które mogą mieć inne znaczenie.
            // W tym przypadku, jeśli "role" zostało usunięte, to jest ok.
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