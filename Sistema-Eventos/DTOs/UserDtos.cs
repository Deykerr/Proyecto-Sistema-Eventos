//Sistema-Eventos\DTOs\UserDtos.cs
using System.ComponentModel.DataAnnotations;

namespace Sistema_Eventos.DTOs
{
    // Lo que el usuario ve de su perfil
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Lo que el usuario puede editar
    public class UpdateProfileDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        // Opcional: Podrías agregar cambio de contraseña aquí, 
        // pero por seguridad suele ir en un endpoint aparte.
    }
}