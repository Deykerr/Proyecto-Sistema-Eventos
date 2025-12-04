using System.ComponentModel.DataAnnotations;

namespace Sistema_Eventos.DTOs
{
    // Para iniciar el pago
    public class PaymentIntentDto
    {
        [Required]
        public Guid ReservationId { get; set; }
    }

    // Respuesta con la URL de pago (simulada)
    public class PaymentResponseDto
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }

    // Lo que nos enviaría "Stripe" o "PayPal" para confirmar
    public class WebhookDto
    {
        public string EventType { get; set; } = string.Empty; // Ej: "payment.succeeded"
        public string TransactionId { get; set; } = string.Empty;
        public Guid ReservationId { get; set; }
    }
}