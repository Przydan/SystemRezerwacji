using System.ComponentModel.DataAnnotations;

namespace SystemRezerwacji.Application.DTOs.Auth;

public class LoginUserDto
{
    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}