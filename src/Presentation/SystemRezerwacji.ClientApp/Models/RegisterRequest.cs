namespace SystemRezerwacji.ClientApp.Models
{
    // SystemRezerwacji.ClientApp/Models/RegisterRequest.cs
    using System.ComponentModel.DataAnnotations;

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Adres email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
        [Compare(nameof(Password), ErrorMessage = "Hasła nie są zgodne.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Opcjonalnie, zgodnie z planem (sekcja 2.1 User):
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}