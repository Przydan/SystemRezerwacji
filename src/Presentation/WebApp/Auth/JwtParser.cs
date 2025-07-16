using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.IdentityModel.JsonWebTokens;

namespace WebApp.Auth;

public static class JwtParser
{
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs == null)
        {
            return claims;
        }

        // Mapowanie standardowych kluczy JWT na standardowe typy oświadczeń ClaimTypes
        if (keyValuePairs.TryGetValue(JwtRegisteredClaimNames.Sub, out var sub))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, sub.ToString() ?? string.Empty));
        }
        else if (keyValuePairs.TryGetValue("nameid", out var nameid))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, nameid.ToString() ?? string.Empty));
        }

        if (keyValuePairs.TryGetValue(ClaimTypes.Email, out var email))
        {
            claims.Add(new Claim(ClaimTypes.Email, email.ToString() ?? string.Empty));
        }

        // Ulepszone i uproszczone parsowanie ról
        if (keyValuePairs.TryGetValue("role", out object? rolesValue))
        {
            if (rolesValue is JsonElement rolesElement && rolesElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in rolesElement.EnumerateArray())
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.GetString() ?? string.Empty));
                }
            }
            else if (rolesValue != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, rolesValue.ToString() ?? string.Empty));
            }
        }
        
        // Dodaj pozostałe claims, które nie są standardowe
        var standardClaimKeys = new[] { "sub", "nameid", "email", "role", "jti", "exp", "iat", "nbf", "iss", "aud" };
        claims.AddRange(keyValuePairs
            .Where(kvp => !standardClaimKeys.Contains(kvp.Key))
            .Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? "")));

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