namespace Shared.DTOs.Auth;

public class AuthResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public DateTime? Expiration { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public IList<string>? Roles { get; set; }
}