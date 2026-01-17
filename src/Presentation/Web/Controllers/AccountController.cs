using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Shared.DTOs.Auth;

namespace Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDto model)
    {
        if (ModelState.IsValid)
        {
            // Security: Enable lockout on failure to prevent brute-force attacks
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Konto zostało tymczasowo zablokowane z powodu zbyt wielu nieudanych prób logowania. Spróbuj ponownie za kilka minut.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nieprawidłowy email lub hasło.");
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequestDto model)
    {
        if (ModelState.IsValid)
        {
            var user = new User { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
