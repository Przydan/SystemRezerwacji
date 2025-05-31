using SystemRezerwacji.WebApp.Models;
using System.Threading.Tasks;
using SystemRezerwacji.Application.DTOs.Auth;

namespace SystemRezerwacji.WebApp.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    Task LogoutAsync();
}