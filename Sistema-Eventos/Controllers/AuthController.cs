using Microsoft.AspNetCore.Mvc;
using Sistema_Eventos.DTOs;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Controllers
{
    [Route("api/v1/auth")] // Prefijo de ruta según especificación del PDF
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/v1/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _authService.RegisterAsync(request);
                // Retornamos 201 Created y, por seguridad, no devolvemos el objeto usuario completo con el hash
                return StatusCode(201, new { message = "Usuario registrado exitosamente", userId = user.Id });
            }
            catch (Exception ex)
            {
                // Manejo básico de errores (ej. si el email ya existe)
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/v1/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return Ok(result);
        }
    }
}