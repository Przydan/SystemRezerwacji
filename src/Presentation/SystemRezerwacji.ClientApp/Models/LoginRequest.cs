namespace SystemRezerwacji.ClientApp.Models
{ 
// SystemRezerwacji.ClientApp/Models/LoginRequest.cs
using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required(ErrorMessage = "Adres email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    public string Password { get; set; } = string.Empty;
}
}
