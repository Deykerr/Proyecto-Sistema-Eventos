using Sistema_Eventos.Models;

namespace Sistema_Eventos.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(Guid id);

        // Requerimiento del PDF: "GET /api/v1/events/my-events"
        Task<List<Event>> GetEventsByOrganizerAsync(Guid organizerId);

        Task AddEventAsync(Event evento);
        Task UpdateEventAsync(Event evento);
        Task DeleteEventAsync(Event evento);
    }
}