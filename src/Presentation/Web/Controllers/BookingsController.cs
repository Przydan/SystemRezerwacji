using Application.Interfaces.Booking;
using Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Shared.DTOs.Booking;

namespace Web.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IResourceService _resourceService;

        public BookingsController(IBookingService bookingService, IResourceService resourceService)
        {
            _bookingService = bookingService;
            _resourceService = resourceService;
        }

        // GET: /Bookings
        public async Task<IActionResult> Index(string sortOrder, string searchString, string statusFilter, Guid? resourceIdFilter)
        {
            ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "Date_desc" : ""; // Default desc
            ViewData["ResourceSortParm"] = sortOrder == "Resource" ? "resource_desc" : "Resource";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["CurrentFilter"] = searchString;
            ViewData["StatusFilter"] = statusFilter;
            ViewData["ResourceIdFilter"] = resourceIdFilter;

            var userId = GetUserIdFromToken();
            var bookingsDto = await _bookingService.GetUserBookingsAsync(userId);
            IEnumerable<BookingDto> bookings = bookingsDto;
            
            // Dropdowns (Populate based on available data or all resources)
            var allResources = await _resourceService.GetAllResourcesAsync();
            ViewBag.Resources = new SelectList(allResources, "Id", "Name", resourceIdFilter);
            ViewBag.Statuses = new SelectList(new List<string> { "Confirmed", "Cancelled", "CancelledByUser", "CancelledByAdmin" }, statusFilter);

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b => b.ResourceName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                            || (b.Notes != null && b.Notes.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
            }
            
            if (!string.IsNullOrEmpty(statusFilter))
            {
                 bookings = bookings.Where(b => b.Status == statusFilter);
            }
            
            if (resourceIdFilter.HasValue)
            {
                bookings = bookings.Where(b => b.ResourceId == resourceIdFilter.Value);
            }

            switch (sortOrder)
            {
                case "Date_desc":
                    bookings = bookings.OrderByDescending(b => b.StartTime);
                    break;
                case "Resource":
                    bookings = bookings.OrderBy(b => b.ResourceName);
                    break;
                case "resource_desc":
                    bookings = bookings.OrderByDescending(b => b.ResourceName);
                    break;
                case "Status":
                    bookings = bookings.OrderBy(b => b.Status);
                    break;
                case "status_desc":
                    bookings = bookings.OrderByDescending(b => b.Status);
                    break;
                default:
                    bookings = bookings.OrderBy(b => b.StartTime);
                    break;
            }

            return View(bookings);
        }
        
        // GET: /Bookings/AdminIndex
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminIndex(string sortOrder, string searchString, string statusFilter, Guid? resourceIdFilter, Guid? userIdFilter)
        {
            ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "Date_desc" : "";
            ViewData["ResourceSortParm"] = sortOrder == "Resource" ? "resource_desc" : "Resource";
            ViewData["UserSortParm"] = sortOrder == "User" ? "user_desc" : "User";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["CurrentFilter"] = searchString;
            ViewData["StatusFilter"] = statusFilter;
            ViewData["ResourceIdFilter"] = resourceIdFilter;
            ViewData["UserIdFilter"] = userIdFilter;

            var bookingsDto = await _bookingService.GetAllBookingsAsync();
            IEnumerable<BookingDto> bookings = bookingsDto;

            // Prepare Dropdowns using distinct values from loaded bookings (to show only relevant options)
            // Or fetch all resources/users. Let's use what we have in booking list for Users, and Service for Resources
            var uniqueUserIds = bookingsDto.Select(b => new { b.UserId, b.UserName }).DistinctBy(u => u.UserId).ToList();
            ViewBag.Users = new SelectList(uniqueUserIds, "UserId", "UserName", userIdFilter);
            
            var allResources = await _resourceService.GetAllResourcesAsync();
             ViewBag.Resources = new SelectList(allResources, "Id", "Name", resourceIdFilter);
             ViewBag.Statuses = new SelectList(new List<string> { "Confirmed", "Cancelled", "CancelledByUser", "CancelledByAdmin" }, statusFilter);

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b => b.ResourceName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                            || (b.UserName != null && b.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                            || (b.Notes != null && b.Notes.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                 bookings = bookings.Where(b => b.Status == statusFilter);
            }
            
            if (resourceIdFilter.HasValue)
            {
                bookings = bookings.Where(b => b.ResourceId == resourceIdFilter.Value);
            }
            
             if (userIdFilter.HasValue)
            {
                bookings = bookings.Where(b => b.UserId == userIdFilter.Value);
            }

            switch (sortOrder)
            {
                case "Date_desc":
                    bookings = bookings.OrderByDescending(b => b.StartTime);
                    break;
                case "Resource":
                    bookings = bookings.OrderBy(b => b.ResourceName);
                    break;
                case "resource_desc":
                    bookings = bookings.OrderByDescending(b => b.ResourceName);
                    break;
                case "User":
                    bookings = bookings.OrderBy(b => b.UserName);
                    break;
                case "user_desc":
                    bookings = bookings.OrderByDescending(b => b.UserName);
                    break;
                 case "Status":
                    bookings = bookings.OrderBy(b => b.Status);
                    break;
                case "status_desc":
                    bookings = bookings.OrderByDescending(b => b.Status);
                    break;
                default:
                    bookings = bookings.OrderBy(b => b.StartTime);
                    break;
            }

            return View(bookings);
        }

        // GET: /Bookings/Create?resourceId=...
        public async Task<IActionResult> Create(Guid? resourceId)
        {
            var resources = await _resourceService.GetActiveResourcesAsync();
            ViewBag.Resources = new SelectList(resources, "Id", "Name", resourceId);
            
            var model = new BookingRequestDto();
            if (resourceId.HasValue)
            {
                model.ResourceId = resourceId.Value;
            }
            // Set default dates if needed, e.g., tomorrow 9:00 - 10:00
            model.StartTime = DateTime.Now.Date.AddDays(1).AddHours(9);
            model.EndTime = DateTime.Now.Date.AddDays(1).AddHours(10);
            
            return View(model);
        }

        // POST: /Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingRequestDto bookingRequest)
        {
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                try 
                {
                    var createdBooking = await _bookingService.CreateBookingAsync(bookingRequest, userId);

                    if (createdBooking == null)
                    {
                        ModelState.AddModelError(string.Empty, "Wybrany termin jest już zajęty.");
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Stale cookie / User ID mismatch after DB reset
                    return RedirectToAction("Logout", "Account");
                }
                catch (KeyNotFoundException)
                {
                    ModelState.AddModelError(string.Empty, "Wybrany zasób nie istnieje.");
                }
            }

            // Reload resources list if validation failed or conflict occurred
            var resources = await _resourceService.GetActiveResourcesAsync();
            ViewBag.Resources = new SelectList(resources, "Id", "Name", bookingRequest.ResourceId);
            return View(bookingRequest);
        }
        
        // POST: /Bookings/Cancel/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id, string? returnUrl = null)
        {
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty) return RedirectToAction("Login", "Account");

            if (User.IsInRole("Administrator"))
            {
                await _bookingService.AdminCancelBookingAsync(id);
                TempData["SuccessMessage"] = "Rezerwacja została anulowana przez Administratora.";
            }
            else
            {
                await _bookingService.CancelBookingAsync(id, userId);
                TempData["SuccessMessage"] = "Rezerwacja została pomyślnie anulowana.";
            }
            
            // Security: Prevent Open Redirect - only allow local URLs
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Default fallback if no returnUrl
            return RedirectToAction(User.IsInRole("Administrator") ? nameof(AdminIndex) : nameof(Index));
        }

        // GET: /Bookings/Calendar
        public IActionResult Calendar()
        {
            return View();
        }

        // GET: /Bookings/GetCalendarEvents
        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents(DateTime start, DateTime end)
        {
            try 
            {
                // Fetch all bookings within range to display availability
                // Note: For a real enterprise app, we should filter by range at DB level. 
                // Currently fetching all and filtering in memory for MVP.
                var allBookings = await _bookingService.GetAllBookingsAsync();
                
                var events = allBookings
                    .Where(b => b.StartTime >= start && b.EndTime <= end && b.Status != "Cancelled" && b.Status != "CancelledByUser" && b.Status != "CancelledByAdmin")
                    .Select(b => new 
                    {
                        id = b.Id,
                        title = $"{b.ResourceName} ({b.UserName})",
                        start = b.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"), // ISO 8601
                        end = b.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        resourceId = b.ResourceId,
                        // Color coding: Blue for others, Green for mine (if logged in)
                        color = (User.Identity?.IsAuthenticated == true && (b.UserId == GetUserIdFromToken())) ? "#28a745" : "#007bff",
                        description = b.Notes
                    });

                return Json(events);
            }
            catch (Exception)
            {
                // Log error
                return BadRequest("Could not load events");
            }
        }
        
        // GET: /Bookings/ExportToIcs/{id}
        [HttpGet]
        public async Task<IActionResult> ExportToIcs(Guid id)
        {
            var userId = GetUserIdFromToken();
            var booking = (await _bookingService.GetAllBookingsAsync()).FirstOrDefault(b => b.Id == id);
            
            if (booking == null) return NotFound();
            
            // Security: Only allow export of own bookings or if admin
            if (booking.UserId != userId && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }
            
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//System Rezerwacji//NONSGML v1.0//EN");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:{booking.Id}");
            sb.AppendLine($"DTSTAMP:{DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ")}");
            sb.AppendLine($"DTSTART:{booking.StartTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ")}");
            sb.AppendLine($"DTEND:{booking.EndTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ")}");
            sb.AppendLine($"SUMMARY:Rezerwacja: {booking.ResourceName}");
            sb.AppendLine($"DESCRIPTION:Rezerwacja zasobu {booking.ResourceName} w Systemie Rezerwacji.");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/calendar", $"rezerwacja_{booking.Id}.ics");
        }

        // POST: /Bookings/UpdateBookingTime
        [HttpPost]
        public async Task<IActionResult> UpdateBookingTime([FromBody] UpdateTimeDto request)
        {
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty) return Unauthorized();

            BookingDto? original;
            bool success;

            if (User.IsInRole("Administrator"))
            {
                 original = await _bookingService.GetBookingByIdForAdminAsync(request.Id);
                 if (original == null) return NotFound("Rezerwacja nie znaleziona.");

                 var updateDto = new UpdateBookingRequestDto
                 {
                     StartTime = request.Start,
                     EndTime = request.End,
                     Notes = original.Notes
                 };

                 success = await _bookingService.AdminUpdateBookingAsync(request.Id, updateDto);
            }
            else
            {
                 original = await _bookingService.GetBookingByIdAsync(request.Id, userId);
                 if (original == null) return NotFound("Rezerwacja nie znaleziona lub brak uprawnień.");

                 var updateDto = new UpdateBookingRequestDto
                 {
                     StartTime = request.Start,
                     EndTime = request.End,
                     Notes = original.Notes
                 };

                 success = await _bookingService.UpdateBookingAsync(request.Id, updateDto, userId);
            }
            
            if (!success)
            {
                 return Conflict("Konflikt terminów lub błąd aktualizacji.");
            }

            return Ok(new { message = "Zaktualizowano termin." });
        }

        public record UpdateTimeDto(Guid Id, DateTime Start, DateTime End);

        private Guid GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var userId))
            {
                return userId;
            }
            return Guid.Empty;
        }
    }
}