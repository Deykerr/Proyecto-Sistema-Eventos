//Sistema-Eventos\Models\Reservation.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Eventos.Models
{
    public class Reservation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public Guid EventId { get; set; }
        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public DateTime ReservationDate { get; set; } = DateTime.UtcNow;

        // Campos opcionales útiles no listados explícitamente pero implícitos en pagos
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
    }
}