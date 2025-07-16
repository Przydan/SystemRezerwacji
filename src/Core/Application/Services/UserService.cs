using Application.Interfaces.User;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.User;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    // Wstrzykujemy UserManager i AutoMapper przez konstruktor
    public UserService(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        // Pobieramy wszystkich użytkowników z bazy danych
        var users = await _userManager.Users.AsNoTracking().ToListAsync();

        // Mapujemy listę encji User na listę obiektów UserDto
        // To wymaga odpowiedniej konfiguracji w profilu AutoMapper
        var userDtos = _mapper.Map<List<UserDto>>(users);

        return userDtos;
    }
}