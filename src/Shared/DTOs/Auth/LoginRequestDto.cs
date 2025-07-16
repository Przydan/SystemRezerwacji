using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Adres email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [DataType(DataType.Password)] // <-- DODANY ATRYBUT
    public string Password { get; set; } = string.Empty;
}