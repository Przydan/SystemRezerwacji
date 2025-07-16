using Shared.DTOs.User;

namespace Application.Interfaces.User;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    // W przyszłości możesz tu dodać inne metody, np. GetUserByIdAsync(Guid id)
}