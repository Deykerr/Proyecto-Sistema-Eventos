using Sistema_Eventos.DTOs;
using Sistema_Eventos.Models;
using Sistema_Eventos.Repositories.Interfaces;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        // Aquí inyectaríamos el servicio externo (IEmailProvider, ISmsProvider) en el futuro

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<NotificationResponseDto>> GetUserNotificationsAsync(Guid userId)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId);
            return notifications.Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                Subject = n.Subject,
                Content = n.Content,
                Type = n.Type.ToString(),
                Status = n.Status.ToString(),
                CreatedAt = n.CreatedAt,
                SentAt = n.SentAt
            }).ToList();
        }

        public async Task SendNotificationAsync(Guid userId, string subject, string content, NotificationType type)
        {
            // 1. Crear el registro en BD (Estado Pendiente)
            var notification = new Notification
            {
                UserId = userId,
                Subject = subject,
                Content = content,
                Type = type,
                Status = NotificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);

            // 2. Simulación de llamada a API Externa (SendGrid/Twilio)
            bool envioExitoso = await SimulateExternalProviderAsync(type, subject);

            // 3. Actualizar estado según resultado
            if (envioExitoso)
            {
                notification.Status = NotificationStatus.Sent;
                notification.SentAt = DateTime.UtcNow;
            }
            else
            {
                notification.Status = NotificationStatus.Failed;
            }

            await _notificationRepository.UpdateAsync(notification);
        }

        // Método auxiliar para simular el envío real
        private async Task<bool> SimulateExternalProviderAsync(NotificationType type, string subject)
        {
            // Simulamos un pequeño delay de red
            await Task.Delay(100);

            // Aquí iría la lógica real:
            // if (type == NotificationType.Email) _emailProvider.Send(...)

            Console.WriteLine($"[SIMULACIÓN] Enviando {type}: {subject}");
            return true; // Siempre exitoso por ahora
        }
    }
}