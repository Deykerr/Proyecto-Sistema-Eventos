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

        // POST: api/v1/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Fíjate que aquí ya NO asignamos el resultado a una variable (var result = ...)
            await _authService.ForgotPasswordAsync(request);

            return Ok(new { message = "Si el correo está registrado, recibirás un token de recuperación." });
        }

        // POST: api/v1/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(request);

            if (!result)
            {
                return BadRequest(new { message = "Token inválido, expirado o error al actualizar." });
            }

            return Ok(new { message = "Contraseña actualizada correctamente. Ya puedes iniciar sesión." });
        }
    }
}