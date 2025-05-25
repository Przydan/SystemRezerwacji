using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SystemRezerwacji.Application.DTOs.Auth;
using SystemRezerwacji.Application.Interfaces.Identity;

namespace SystemRezerwacji.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = "Niepoprawne dane wejściowe.", Roles = new List<string>() });
            }

            var result = await _authService.RegisterUserAsync(registerDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            // Po pomyślnej rejestracji nie zwracamy tokenu, użytkownik musi się zalogować
            return Ok(new AuthResponseDto { IsSuccess = true, Message = "Rejestracja zakończona pomyślnie. Możesz się teraz zalogować." });
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = "Niepoprawne dane wejściowe.", Roles = new List<string>() });
            }

            var result = await _authService.LoginUserAsync(loginDto);
            if (!result.IsSuccess)
            {
                // Zgodnie z praktyką, dla nieudanego logowania zwracamy ogólny błąd bez szczegółów,
                // ale dla celów developmentu można zwrócić pełniejszy komunikat z AuthService.
                return Unauthorized(new AuthResponseDto { IsSuccess = false, Message = result.Message, Roles = new List<string>() });
            }
            return Ok(result);
        }
    }
}