using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Shared.DTOs.Booking;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Większość metod w tym kontrolerze prawdopodobnie będzie wymagać autoryzacji
    public class BookingsController : ControllerBase
    {
        // Przykład wstrzyknięcia serwisu aplikacyjnego (dostosuj do swojej architektury)
        // private readonly IBookingApplicationService _bookingAppService;
        // public BookingsController(IBookingApplicationService bookingAppService)
        // {
        //     _bookingAppService = bookingAppService;
        // }

        [HttpGet("my")] // Definiuje ścieżkę GET api/bookings/my
        [ProducesResponseType(typeof(List<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Jeśli użytkownik nie ma rezerwacji lub błąd
        public async Task<ActionResult<List<BookingDto>>> GetMyBookings()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                // Ten błąd nie powinien wystąpić, jeśli jest [Authorize], ale dla pewności
                return Unauthorized("User ID not found in token.");
            }

            Console.WriteLine($"API Endpoint: GetMyBookings called for User ID: {userId}"); // Log serwera

            // TODO: Zaimplementuj logikę pobierania rezerwacji dla 'userId'
            // Przykład z użyciem hipotetycznego serwisu aplikacyjnego:
            // var bookings = await _bookingAppService.GetBookingsForUserAsync(userId);
            // if (bookings == null) // Lub jeśli serwis rzuca wyjątek, który jest tu łapany
            // {
            //     return NotFound("No bookings found for the user or error occurred.");
            // }
            // return Ok(bookings);

            // ---- TYMCZASOWA ODPOWIEDŹ DLA TESTÓW KLIENTA (Zastąp powyższą logiką) ----
            // Zwróć pustą listę, aby klient nie dostawał 404, dopóki nie zaimplementujesz logiki.
            // Po implementacji usuń tę tymczasową odpowiedź.
            var dummyBookings = new List<BookingDto>
            {
                new BookingDto
                {
                    Id = Guid.NewGuid(), ResourceName = "Zasób Testowy 1 (z API)",
                    StartTime = DateTime.UtcNow.AddHours(2), EndTime = DateTime.UtcNow.AddHours(3),
                    Status = "Confirmed", UserName = "Test User API", UserId = userId, ResourceId = Guid.NewGuid()
                },
                new BookingDto
                {
                    Id = Guid.NewGuid(), ResourceName = "Zasób Testowy 2 (z API)",
                    StartTime = DateTime.UtcNow.AddDays(1), EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
                    Status = "PendingApproval", UserName = "Test User API", UserId = userId, ResourceId = Guid.NewGuid()
                }
            };
            return Ok(dummyBookings);
            // ---- KONIEC TYMCZASOWEJ ODPOWIEDZI ----
        }

        // Tutaj inne metody kontrolera, np. POST api/bookings do tworzenia rezerwacji
        [HttpPost]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] BookingRequestDto bookingRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            Console.WriteLine(
                $"API Endpoint: CreateBooking called by User ID: {userId} for Resource ID: {bookingRequest.ResourceId}");

            // TODO: Logika tworzenia rezerwacji w serwisie aplikacyjnym
            // BookingDto createdBooking = await _bookingAppService.CreateBookingAsync(bookingRequest, userId);
            // return CreatedAtAction(nameof(GetBookingById), new { id = createdBooking.Id }, createdBooking); // Załóżmy, że masz GetBookingById

            // ---- TYMCZASOWA ODPOWIEDŹ DLA TESTÓW KLIENTA ----
            var dummyCreatedBooking = new BookingDto
            {
                Id = Guid.NewGuid(),
                ResourceId = bookingRequest.ResourceId,
                UserId = userId,
                ResourceName = "Zasób X (API)",
                UserName = "Użytkownik Y (API)",
                StartTime = bookingRequest.StartTime,
                EndTime = bookingRequest.EndTime,
                Status = "Confirmed",
                Notes = bookingRequest.Notes
            };
            return Ok(dummyCreatedBooking); // Prostsze na razie
        }
    }
}