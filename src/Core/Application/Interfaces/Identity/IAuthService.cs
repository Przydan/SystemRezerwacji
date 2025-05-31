

using Shared.DTOs.Auth;

namespace Application.Interfaces.Identity;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterUserAsync(RegisterUserDto registerDto, string defaultRole = "User");
    Task<AuthResponseDto> LoginUserAsync(LoginUserDto loginDto);
    // W przyszłości można dodać:
    // Task <AuthResponseDto> RefreshTokenAsync(string token);
    // Task LogoutAsync(string userId);
    Task Logout();
}