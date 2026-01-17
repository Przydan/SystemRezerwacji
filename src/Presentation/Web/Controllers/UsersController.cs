using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Web.ViewModels;
using Shared.DTOs.Auth;

namespace Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private const string PrimaryAdminEmail = "admin@x.pl";
        
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Checks if the currently logged-in user is the primary administrator.
        /// </summary>
        private bool IsPrimaryAdmin() => User.Identity?.Name == PrimaryAdminEmail;

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

            ViewBag.IsPrimaryAdmin = IsPrimaryAdmin();
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

        // ===============================
        // PASSWORD MANAGEMENT (Primary Admin Only)
        // ===============================

        /// <summary>
        /// GET: Display form for the primary admin to change their own password.
        /// </summary>
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!IsPrimaryAdmin())
            {
                TempData["ErrorMessage"] = "Brak uprawnień do tej akcji.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ChangePasswordDto());
        }

        /// <summary>
        /// POST: Process the primary admin's password change.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!IsPrimaryAdmin())
            {
                TempData["ErrorMessage"] = "Brak uprawnień do tej akcji.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Hasło zostało zmienione pomyślnie.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        /// <summary>
        /// GET: Display form for the primary admin to reset another user's password.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ResetPassword(Guid userId)
        {
            if (!IsPrimaryAdmin())
            {
                TempData["ErrorMessage"] = "Brak uprawnień do tej akcji.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Don't allow resetting own password via this action
            if (user.UserName == PrimaryAdminEmail)
            {
                TempData["ErrorMessage"] = "Użyj opcji 'Zmień hasło' aby zmienić własne hasło.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ResetUserPasswordDto
            {
                UserId = user.Id,
                UserEmail = user.Email ?? user.UserName ?? ""
            };

            return View(model);
        }

        /// <summary>
        /// POST: Process resetting another user's password.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetUserPasswordDto model)
        {
            if (!IsPrimaryAdmin())
            {
                TempData["ErrorMessage"] = "Brak uprawnień do tej akcji.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Don't allow resetting own password via this action
            if (user.UserName == PrimaryAdminEmail)
            {
                TempData["ErrorMessage"] = "Użyj opcji 'Zmień hasło' aby zmienić własne hasło.";
                return RedirectToAction(nameof(Index));
            }

            // Remove old password and set new one
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Hasło użytkownika {user.Email} zostało zresetowane.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}