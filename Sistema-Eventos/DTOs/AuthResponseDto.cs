using System.ComponentModel.DataAnnotations;

namespace Sistema_Eventos.DTOs
{
    // Lo que tu API responde cuando el login es exitoso
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty; // El JWT
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}