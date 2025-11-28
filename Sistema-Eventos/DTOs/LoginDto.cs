using System.ComponentModel.DataAnnotations;

namespace Sistema_Eventos.DTOs
{

    // Lo que el usuario envía para entrar
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

}