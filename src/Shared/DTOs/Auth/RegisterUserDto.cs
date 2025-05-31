using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Auth;

public class RegisterUserDto
{
    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Hasło musi mieć co najmniej 8 znaków.")]
    // Zgodnie z planem, polityka haseł: RequireDigit=true, RequiredLength=8, RequireUppercase=true, RequireLowercase=true
    // Tutaj można dodać bardziej szczegółowe [RegularExpression] lub pozostawić to dla FluentValidation
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Potwierdź hasło")]
    [Compare("Password", ErrorMessage = "Hasło i jego potwierdzenie nie są zgodne.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [StringLength(50)] public string? FirstName { get; set; }

    [StringLength(50)] public string? LastName { get; set; }
}