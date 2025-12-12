using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Eventos.DTOs;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    [Authorize] // Todo requiere login
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/v1/users/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Extraer ID del token
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var profile = await _userService.GetProfileAsync(userId);
            if (profile == null) return NotFound();

            return Ok(profile);
        }

        // PUT: api/v1/users/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            try
            {
                var updatedProfile = await _userService.UpdateProfileAsync(userId, dto);
                if (updatedProfile == null) return NotFound();

                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- ENDPOINTS PARA ADMIN ---

        // GET: api/v1/users (Listar todos)
        [HttpGet]
        [Authorize(Roles = "Admin")] // <--- SOLO ADMIN
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/v1/users/{id} (Ver uno específico)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAdminAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // PUT: api/v1/users/{id}/status (Banear/Desbanear)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var success = await _userService.ToggleUserStatusAsync(id);
            if (!success) return NotFound();

            return Ok(new { message = "El estado del usuario ha sido actualizado." });
        }
    }
}