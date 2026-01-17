using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Booking;
using Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Web.ViewModels;
using System.Security.Claims;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IResourceService _resourceService;
    private readonly UserManager<User> _userManager;

    public HomeController(IBookingService bookingService, IResourceService resourceService, UserManager<User> userManager)
    {
        _bookingService = bookingService;
        _resourceService = resourceService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel();

        if (User.IsInRole("Administrator"))
        {
            // Admin Stats
            model.TotalUsers = _userManager.Users.Count();
            
            var allResources = await _resourceService.GetAllResourcesAsync();
            model.TotalResources = allResources.Count();

            var allBookings = await _bookingService.GetAllBookingsAsync();
            var today = DateTime.Today;
            model.ActiveBookingsToday = allBookings
                .Count(b => b.StartTime.Date == today && b.Status == "Confirmed");
            
            // Allow Admin to see recent bookings too? Maybe unrelated.
        }
        else if (User.Identity?.IsAuthenticated == true)
        {
            // User Data
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                var myBookings = await _bookingService.GetUserBookingsAsync(userId);
                model.UpcomingBookings = myBookings
                    .Where(b => b.EndTime > DateTime.Now && b.Status == "Confirmed")
                    .OrderBy(b => b.StartTime)
                    .Take(3)
                    .ToList();
            }
        }

        return View(model);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
