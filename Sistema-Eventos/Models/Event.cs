//Sistema-Eventos\Models\Event.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Eventos.Models
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // Claves foráneas
        [Required]
        public Guid OrganizerId { get; set; }
        [ForeignKey("OrganizerId")]
        public User? Organizer { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        //[cite_start]// Coordenadas para geolocalización (decimal) [cite: 115]
        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }

        public int Capacity { get; set; }
        public int AvailableSlots { get; set; } // Franjas horarias o cupos disponibles

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public bool IsPublic { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
    }
}