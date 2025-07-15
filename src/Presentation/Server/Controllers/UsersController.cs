using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")] // Zabezpieczenie - tylko dla admina
public class UsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public UsersController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    // Ten endpoint będzie odpowiadał na żądanie GET: /api/users
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        // Pobieramy z bazy listę użytkowników, wybierając tylko te pola, które są nam potrzebne
        var users = await _userManager.Users
            .Select(u => new { u.Id, u.Email, u.FirstName, u.LastName })
            .ToListAsync();
            
        return Ok(users);
    }
}