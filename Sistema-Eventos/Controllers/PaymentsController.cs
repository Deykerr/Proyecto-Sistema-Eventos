using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Eventos.DTOs;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/v1/payments/create-intent
        // El usuario pide pagar su reserva
        [HttpPost("create-intent")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentIntentDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            try
            {
                var response = await _paymentService.CreatePaymentLinkAsync(userId, dto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/v1/payments/webhook
        // Simula la llamada que haría PayPal/Stripe a nuestro servidor para avisar que el pago pasó.
        // NO requiere Authorize porque los webhooks son públicos (se validan por firma, aquí simplificado).
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] WebhookDto dto)
        {
            var success = await _paymentService.ProcessWebhookAsync(dto);

            if (!success) return BadRequest(new { message = "No se pudo procesar el pago o reserva no encontrada" });

            return Ok(new { message = "Webhook procesado correctamente" });
        }
    }
}