using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Auth;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Aktualne hasło jest wymagane.")]
    [DataType(DataType.Password)]
    [Display(Name = "Aktualne hasło")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nowe hasło jest wymagane.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Hasło musi mieć co najmniej 8 znaków.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nowe hasło")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
    [DataType(DataType.Password)]
    [Display(Name = "Potwierdź nowe hasło")]
    [Compare("NewPassword", ErrorMessage = "Hasło i jego potwierdzenie nie są zgodne.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
