// SystemRezerwacji.ClientApp/Services/IAuthService.cs
using System.Threading.Tasks;
using SystemRezerwacji.ClientApp.Models; // Założenie, że modele DTO będą tutaj

public interface IAuthService
{
    Task<bool> Login(LoginRequest model);
    Task<bool> Register(RegisterRequest model);
    Task Logout();
    Task<string?> GetToken(); // Do pobierania tokenu, np. dla HttpClient
}
