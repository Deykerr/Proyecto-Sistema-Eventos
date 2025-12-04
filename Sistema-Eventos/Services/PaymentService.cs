using Sistema_Eventos.DTOs;
using Sistema_Eventos.Models;
using Sistema_Eventos.Repositories.Interfaces;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly INotificationService _notificationService;

        public PaymentService(IReservationRepository reservationRepository, INotificationService notificationService)
        {
            _reservationRepository = reservationRepository;
            _notificationService = notificationService;
        }

        public async Task<PaymentResponseDto> CreatePaymentLinkAsync(Guid userId, PaymentIntentDto dto)
        {
            var reservation = await _reservationRepository.GetByIdAsync(dto.ReservationId);

            if (reservation == null) throw new Exception("Reserva no encontrada");
            if (reservation.UserId != userId) throw new UnauthorizedAccessException("La reserva no te pertenece");
            if (reservation.Status != ReservationStatus.Pending) throw new Exception("La reserva no está pendiente de pago");

            // Simulación: Generamos un ID de transacción falso
            var transactionId = Guid.NewGuid().ToString();

            // En un caso real, aquí llamaríamos a la API de Stripe/PayPal
            return new PaymentResponseDto
            {
                TransactionId = transactionId,
                // Esta URL es de mentira, pero sirve para el frontend
                PaymentUrl = $"https://paypal-simulado.com/pay?id={transactionId}&amount={reservation.TotalAmount}"
            };
        }

        public async Task<bool> ProcessWebhookAsync(WebhookDto dto)
        {
            // Validamos que sea un evento de pago exitoso
            if (dto.EventType != "payment.succeeded") return false;

            var reservation = await _reservationRepository.GetByIdAsync(dto.ReservationId);
            if (reservation == null) return false;

            if (reservation.Status == ReservationStatus.Confirmed) return true; // Ya estaba confirmada

            // 1. Actualizar estado de la reserva
            reservation.Status = ReservationStatus.Confirmed;
            // Aquí podríamos guardar el TransactionId en la reserva si tuviéramos el campo

            await _reservationRepository.UpdateAsync(reservation);

            // 2. Enviar notificación de éxito (RF06 - Confirmación de Reserva)
            await _notificationService.SendNotificationAsync(
                reservation.UserId,
                "¡Pago Exitoso! Entradas confirmadas",
                $"Tu pago para el evento '{reservation.Event?.Title}' fue procesado correctamente. Tu reserva está confirmada.",
                NotificationType.Email
            );

            return true;
        }
    }
}