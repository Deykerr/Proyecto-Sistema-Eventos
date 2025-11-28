using Sistema_Eventos.DTOs;
using Sistema_Eventos.Repositories.Interfaces;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;

        public ReportService(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(Guid organizerId)
        {
            return await _repository.GetOrganizerStatsAsync(organizerId);
        }

        public async Task<List<EventStatsDto>> GetEventsStatsAsync(Guid organizerId)
        {
            return await _repository.GetEventsStatsAsync(organizerId);
        }
    }
}