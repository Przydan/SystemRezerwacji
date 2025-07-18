using Shared.DTOs.Auth;

namespace WebApp.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    Task LogoutAsync();
}