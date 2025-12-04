using Sistema_Eventos.DTOs;

namespace Sistema_Eventos.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentLinkAsync(Guid userId, PaymentIntentDto dto);
        Task<bool> ProcessWebhookAsync(WebhookDto dto);
    }
}