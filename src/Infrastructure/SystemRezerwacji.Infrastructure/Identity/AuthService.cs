using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SystemRezerwacji.Application.DTOs.Auth;
using SystemRezerwacji.Application.Interfaces.Identity;
using SystemRezerwacji.Domain.Entities;


public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _configuration;
    
    
    public AuthService(
        UserManager<User> userManager, 
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterUserAsync(RegisterUserDto registerDto, string defaultRole = "User")
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Użytkownik o podanym adresie email już istnieje." };
        }

        var newUser = new User
        {
            Email = registerDto.Email,
            UserName = registerDto.Email, // Użyj emaila jako nazwy użytkownika lub zdefiniuj inaczej
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            EmailConfirmed = true // Dla uproszczenia, w produkcji warto zaimplementować potwierdzenie email
        };

        var result = await _userManager.CreateAsync(newUser, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return new AuthResponseDto { IsSuccess = false, Message = string.Join("\n", errors) };
        }

        // Przypisanie domyślnej roli
        if (!string.IsNullOrWhiteSpace(defaultRole))
        {
            if (await _roleManager.RoleExistsAsync(defaultRole))
            {
                await _userManager.AddToRoleAsync(newUser, defaultRole);
            }
            else
            {
                // Można zalogować ostrzeżenie, że domyślna rola nie istnieje
            }
        }

        return new AuthResponseDto { IsSuccess = true, Message = "Rejestracja zakończona pomyślnie." };
    }

    public async Task<AuthResponseDto> LoginUserAsync(LoginUserDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Niepoprawny email lub hasło." };
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Standardowy claim dla ID użytkownika
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unikalny identyfikator tokenu
            // Dodajemy imię i nazwisko do claims, jeśli istnieją
            new Claim(JwtRegisteredClaimNames.Name,
                user.UserName ?? string.Empty) // Lub user. Email, jeśli Username nie jest unikalny
        };
        if (!string.IsNullOrWhiteSpace(user.FirstName))
            authClaims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
        if (!string.IsNullOrWhiteSpace(user.LastName))
            authClaims.Add(new Claim(ClaimTypes.Surname, user.LastName));


        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var jwtKey = _configuration["JwtSettings:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Klucz JWT nie jest skonfigurowany." };
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var tokenValidityInMinutes = int.Parse(_configuration.GetSection("JwtSettings:DurationInMinutes").Value ?? "60");
            
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            Expires = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(authClaims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Logowanie pomyślne.",
            Token = tokenHandler.WriteToken(token),
            Expiration = token.ValidTo,
            UserId = user.Id.ToString(),
            Email = user.Email,
            Roles = userRoles.ToList()
        };
    }

    public Task Logout()
    {
        throw new NotImplementedException();
    }
}