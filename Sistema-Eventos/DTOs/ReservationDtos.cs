using System.ComponentModel.DataAnnotations;

namespace Sistema_Eventos.DTOs
{
    public class CreateReservationDto
    {
        [Required]
        public Guid EventId { get; set; }
    }

    public class ReservationResponseDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty; // Para que el organizador sepa quién es
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}