using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Eventos.DTOs;
using Sistema_Eventos.Models; // Para acceder a Enums si hace falta
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Controllers
{
    [Route("api/v1/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/v1/events
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        // GET: api/v1/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var evento = await _eventService.GetEventByIdAsync(id);
            if (evento == null) return NotFound(new { message = "Evento no encontrado" });
            return Ok(evento);
        }

        // GET: api/v1/events/my-events (Requiere login)
        [HttpGet("my-events")]
        [Authorize(Roles = "Organizer,Admin")] // Solo organizadores o admins
        public async Task<IActionResult> GetMyEvents()
        {
            // Extraer ID del token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);
            var events = await _eventService.GetMyEventsAsync(userId);
            return Ok(events);
        }

        // POST: api/v1/events (Crear)
        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            try
            {
                var createdEvent = await _eventService.CreateEventAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, createdEvent);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/v1/events/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);
            var isAdmin = roleClaim?.Value == "Admin";

            try
            {
                var updatedEvent = await _eventService.UpdateEventAsync(id, dto, userId, isAdmin);
                if (updatedEvent == null) return NotFound();
                return Ok(updatedEvent);
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

        // DELETE: api/v1/events/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);
            var isAdmin = roleClaim?.Value == "Admin";

            try
            {
                var deleted = await _eventService.DeleteEventAsync(id, userId, isAdmin);
                if (!deleted) return NotFound(); // O no se encontró o no se pudo borrar
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}