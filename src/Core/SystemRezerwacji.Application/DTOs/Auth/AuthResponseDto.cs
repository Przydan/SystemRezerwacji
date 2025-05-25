namespace SystemRezerwacji.Application.DTOs.Auth;

public class AuthResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public DateTime? Expiration { get; set; } // Kiedy token wygasa
    public string? UserId { get; set; } // ID zalogowanego użytkownika
    public string? Email { get; set; } // Email zalogowanego użytkownika
    public IList<string>? Roles { get; set; } // Role użytkownika
}