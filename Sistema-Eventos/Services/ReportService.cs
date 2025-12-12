using Sistema_Eventos.DTOs;
using Sistema_Eventos.Repositories.Interfaces;
using Sistema_Eventos.Services.Interfaces;
using System.Text;

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

        public async Task<byte[]> ExportStatsToCsvAsync(Guid organizerId)
        {
            // 1. Obtenemos los datos
            var stats = await _repository.GetEventsStatsAsync(organizerId);

            // 2. Construimos el CSV usando StringBuilder
            var builder = new StringBuilder();

            // Cabecera
            builder.AppendLine("ID Evento,Titulo,Reservas Totales,Ocupacion (%),Ingresos (S/)");

            // Filas
            foreach (var item in stats)
            {
                // Escapamos comas en el título por si acaso
                string safeTitle = item.EventTitle.Contains(",") ? $"\"{item.EventTitle}\"" : item.EventTitle;

                builder.AppendLine($"{item.EventId},{safeTitle},{item.TotalReservations},{item.OccupancyRate:F2},{item.Revenue:F2}");
            }

            // 3. Convertimos a bytes
            return Encoding.UTF8.GetBytes(builder.ToString());
        }
    }
}