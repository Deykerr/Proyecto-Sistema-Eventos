using Sistema_Eventos.DTOs;

namespace Sistema_Eventos.Repositories.Interfaces
{
    public interface IReportRepository
    {
        // Estadísticas generales para un organizador
        Task<DashboardStatsDto> GetOrganizerStatsAsync(Guid organizerId);

        // Estadísticas por evento específico
        Task<List<EventStatsDto>> GetEventsStatsAsync(Guid organizerId);
    }
}