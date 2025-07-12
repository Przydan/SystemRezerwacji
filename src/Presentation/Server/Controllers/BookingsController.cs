using Application.Interfaces.Booking; // Dodaj ten using
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Shared.DTOs.Booking;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService; // Pole do przechowywania serwisu

        // Wstrzyknięcie serwisu przez konstruktor
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("my")]
        [ProducesResponseType(typeof(List<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<BookingDto>>> GetMyBookings()
        {
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            // Jedna linia, która robi całą magię!
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            
            return Ok(bookings);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)] // Nowy kod odpowiedzi dla konfliktu
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] BookingRequestDto bookingRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            var createdBooking = await _bookingService.CreateBookingAsync(bookingRequest, userId);

            if (createdBooking == null)
            {
                // Serwis zwrócił null, co oznacza konflikt terminów.
                return Conflict(new { message = "The selected time slot is already booked." });
            }
            
            return CreatedAtAction(nameof(GetBookingById), new { bookingId = createdBooking.Id }, createdBooking);

        }
        
        [HttpDelete("{bookingId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            var success = await _bookingService.CancelBookingAsync(bookingId, userId);

            if (!success)
            {
                // Jeśli serwis zwrócił false, oznacza to, że rezerwacji nie znaleziono
                // lub nie należy ona do tego użytkownika. Zwracamy 404 Not Found dla bezpieczeństwa.
                return NotFound();
            }

            // Zgodnie ze standardem REST, operacja DELETE, która się powiodła,
            // powinna zwrócić status 204 No Content.
            return NoContent();
        }
        
        [HttpGet("{bookingId:guid}", Name = "GetBookingById")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingDto>> GetBookingById(Guid bookingId)
        {
            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            var booking = await _bookingService.GetBookingByIdAsync(bookingId, userId);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }
        
        [HttpPut("{bookingId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateBooking(Guid bookingId, [FromBody] UpdateBookingRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromToken();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID not found in token.");
            }

            var success = await _bookingService.UpdateBookingAsync(bookingId, request, userId);

            if (!success)
            {
                // Jeśli serwis zwrócił false, może to oznaczać, że rezerwacji nie znaleziono
                // lub wystąpił konflikt. Zwracamy ogólny błąd 404/409.
                // Dla uproszczenia zwrócimy NotFound.
                return NotFound(new { message = "Booking not found or update resulted in a conflict." });
            }

            // Operacja PUT, która się powiodła, powinna zwrócić status 204 No Content.
            return NoContent();
        }

        private Guid GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdString, out var userId);
            return userId;
        }
    }
}