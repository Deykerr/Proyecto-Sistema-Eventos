using Sistema_Eventos.DTOs;
using Sistema_Eventos.Models;

namespace Sistema_Eventos.Services.Interfaces
{
    public interface INotificationService
    {
        // Método para que el usuario vea su historial
        Task<List<NotificationResponseDto>> GetUserNotificationsAsync(Guid userId);

        // Método interno para enviar notificaciones desde otros servicios
        Task SendNotificationAsync(Guid userId, string subject, string content, NotificationType type);
    }
}