using Sistema_Eventos.Models;

namespace Sistema_Eventos.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(Guid id);

        // Para que el usuario vea sus propias reservas
        Task<List<Reservation>> GetByUserIdAsync(Guid userId);

        // Para que el organizador vea quiénes reservaron su evento
        Task<List<Reservation>> GetByEventIdAsync(Guid eventId);

        Task AddAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
    }
}