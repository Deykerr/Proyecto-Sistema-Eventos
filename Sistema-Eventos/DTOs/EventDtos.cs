using Sistema_Eventos.Models; // O Sistema_Eventos.Models
using System.ComponentModel.DataAnnotations;

namespace Sistema_Eventos.DTOs
{
    // Datos necesarios para CREAR un evento
    public class CreateEventDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        // Coordenadas opcionales pero recomendadas
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser al menos 1")]
        public int Capacity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsPublic { get; set; } = true;
    }

    // Datos para ACTUALIZAR (todos opcionales o requeridos según lógica, aquí requeridos para simplificar)
    public class UpdateEventDto : CreateEventDto
    {
        public EventStatus Status { get; set; } // Permitir cambiar estado (Borrador -> Publicado)
    }

    // Lo que devuelve la API (incluye nombres en lugar de solo IDs)
    public class EventResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OrganizerName { get; set; } = string.Empty; // Nombre del organizador
        public string CategoryName { get; set; } = string.Empty;  // Nombre de la categoría
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int AvailableSlots { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
    }
}