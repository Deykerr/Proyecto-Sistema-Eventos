//Sistema-Eventos\DTOs\ResetPasswordDto.cs
using System.ComponentModel.DataAnnotations;

namespace Sistema_Eventos.DTOs
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; } = string.Empty; // El token que recibió por correo

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}