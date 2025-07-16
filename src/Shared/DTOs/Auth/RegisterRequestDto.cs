using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Hasło musi mieć co najmniej 8 znaków.")]
    [DataType(DataType.Password)] // <-- DODANY ATRYBUT dla spójności
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Potwierdź hasło")] // <-- DODANY ATRYBUT
    [Compare("Password", ErrorMessage = "Hasło i jego potwierdzenie nie są zgodne.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [StringLength(50)] // <-- DODANY ATRYBUT
    public string? FirstName { get; set; }

    [StringLength(50)] // <-- DODANY ATRYBUT
    public string? LastName { get; set; }
}