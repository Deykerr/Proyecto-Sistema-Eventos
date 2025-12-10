//Sistema-Eventos\Models\User.cs
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Eventos.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // Único

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty; // Nombre

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty; // Apellido

        [Required]
        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Relaciones (Navegación)
        public ICollection<Event>? OrganizedEvents { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
    }
}