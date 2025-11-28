using Sistema_Eventos.DTOs;

namespace Sistema_Eventos.Services.Interfaces
{
    public interface IReportService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync(Guid organizerId);
        Task<List<EventStatsDto>> GetEventsStatsAsync(Guid organizerId);
    }
}