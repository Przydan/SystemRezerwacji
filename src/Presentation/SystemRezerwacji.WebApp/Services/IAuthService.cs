using SystemRezerwacji.WebApp.Models;
using System.Threading.Tasks;

namespace SystemRezerwacji.WebApp.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    Task LogoutAsync();
}