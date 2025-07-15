using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Identity;
using Domain.Entities;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs.Auth;

namespace Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly JwtSettings _jwtSettings;

    // POPRAWIONY KONSTRUKTOR - zawiera wszystkie potrzebne zależności
    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
    }

    // POPRAWIONA METODA LOGOWANIA - używa SignInManager do obsługi blokady
    public async Task<AuthResponseDto> LoginUserAsync(LoginRequestDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Nieprawidłowy email lub hasło." };
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Konto jest tymczasowo zablokowane z powodu zbyt wielu nieudanych prób logowania." };
        }
        
        if (!result.Succeeded)
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Nieprawidłowy email lub hasło." };
        }
        
        var userRoles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, userRoles);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Logowanie pomyślne.",
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo,
            UserId = user.Id.ToString(),
            Email = user.Email,
            Roles = userRoles.ToList()
        };
    }

    public async Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto registerDto, string defaultRole = "User")
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Użytkownik o podanym adresie email już istnieje." };
        }

        var newUser = new User
        {
            Email = registerDto.Email,
            UserName = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            EmailConfirmed = true 
        };

        var result = await _userManager.CreateAsync(newUser, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return new AuthResponseDto { IsSuccess = false, Message = string.Join("\n", errors) };
        }
        
        if (!string.IsNullOrWhiteSpace(defaultRole) && await _roleManager.RoleExistsAsync(defaultRole))
        {
            await _userManager.AddToRoleAsync(newUser, defaultRole);
        }

        return new AuthResponseDto { IsSuccess = true, Message = "Rejestracja zakończona pomyślnie." };
    }
    
    public Task Logout()
    {
        throw new NotImplementedException();
    }

    // Prywatna metoda pomocnicza do generowania tokenu
    private SecurityToken GenerateJwtToken(User user, IList<string> userRoles)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? throw new InvalidOperationException()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty)
        };

        if (!string.IsNullOrWhiteSpace(user.FirstName))
            authClaims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
        if (!string.IsNullOrWhiteSpace(user.LastName))
            authClaims.Add(new Claim(ClaimTypes.Surname, user.LastName));

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Expires = DateTime.UtcNow.AddMinutes(15), 
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(authClaims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.CreateToken(tokenDescriptor);
    }
}