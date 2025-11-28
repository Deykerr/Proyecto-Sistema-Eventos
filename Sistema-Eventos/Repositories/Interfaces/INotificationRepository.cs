using Sistema_Eventos.Models;

namespace Sistema_Eventos.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
    }
}