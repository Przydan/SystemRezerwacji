using Application.Interfaces.User; 
using Shared.DTOs.User;
using System.Net.Http.Json;    

namespace WebApp.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            // Wywołujemy endpoint API, który stworzyliśmy w kroku 1
            var users = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/users");
            return users ?? new List<UserDto>();
        }
    }
}