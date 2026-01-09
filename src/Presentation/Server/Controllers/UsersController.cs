using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using SystemRezerwacji.Server.ViewModels;

namespace SystemRezerwacji.Server.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var viewModel = new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    IsLockedOut = await _userManager.IsLockedOutAsync(user)
                };

                viewModel.IsAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
                userViewModels.Add(viewModel);
            }

            return View(userViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdmin(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Administrator"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Administrator");
                TempData["SuccessMessage"] = $"Odebrano uprawnienia administratora użytkownikowi {user.UserName}.";
            }
            else
            {
                // Ensure role exists
                if (!await _roleManager.RoleExistsAsync("Administrator"))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>("Administrator"));
                }
                
                await _userManager.AddToRoleAsync(user, "Administrator");
                TempData["SuccessMessage"] = $"Nadano uprawnienia administratora użytkownikowi {user.UserName}.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLockout(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            if (await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["SuccessMessage"] = $"Odblokowano użytkownika {user.UserName}.";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData["SuccessMessage"] = $"Zablokowano użytkownika {user.UserName}.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}