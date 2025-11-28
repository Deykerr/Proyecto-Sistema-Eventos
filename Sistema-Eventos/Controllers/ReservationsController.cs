using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Eventos.DTOs;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Controllers
{
    [Route("api/v1/reservations")]
    [ApiController]
    [Authorize] // Todos estos endpoints requieren estar logueado
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // POST: api/v1/reservations (Crear reserva)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            try
            {
                var result = await _reservationService.CreateReservationAsync(userId, dto);
                return CreatedAtAction(nameof(GetMyReservations), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/v1/reservations/my-reservations (Historial del usuario)
        [HttpGet("my-reservations")]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var list = await _reservationService.GetMyReservationsAsync(userId);
            return Ok(list);
        }

        // PUT: api/v1/reservations/{id}/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            try
            {
                var success = await _reservationService.CancelReservationAsync(id, userId);
                if (!success) return NotFound();
                return Ok(new { message = "Reserva cancelada correctamente." });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint extra útil: Ver reservas de un evento (Para Organizadores)
        // GET: api/v1/events/{eventId}/reservations
        [HttpGet("/api/v1/events/{eventId}/reservations")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> GetEventReservations(Guid eventId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            // Nota: Aquí deberíamos validar que el usuario sea el dueño del evento en el servicio
            try
            {
                var list = await _reservationService.GetReservationsByEventAsync(eventId, userId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}