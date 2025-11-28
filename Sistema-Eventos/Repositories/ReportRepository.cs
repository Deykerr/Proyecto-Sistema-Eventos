using Microsoft.EntityFrameworkCore;
using Sistema_Eventos.Data;
using Sistema_Eventos.DTOs;
using Sistema_Eventos.Models;
using Sistema_Eventos.Repositories.Interfaces;

namespace Sistema_Eventos.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetOrganizerStatsAsync(Guid organizerId)
        {
            // Filtramos eventos del organizador
            var events = _context.Events.Where(e => e.OrganizerId == organizerId);

            // Filtramos reservas asociadas a esos eventos
            var reservations = _context.Reservations.Where(r => r.Event != null && r.Event.OrganizerId == organizerId);

            return new DashboardStatsDto
            {
                TotalEvents = await events.CountAsync(),
                ActiveEvents = await events.CountAsync(e => e.EndDate > DateTime.UtcNow),
                TotalReservations = await reservations.CountAsync(),
                // Sumamos el TotalAmount de las reservas que NO estén canceladas
                TotalRevenue = await reservations
                    .Where(r => r.Status != ReservationStatus.Canceled)
                    .SumAsync(r => r.TotalAmount)
            };
        }

        public async Task<List<EventStatsDto>> GetEventsStatsAsync(Guid organizerId)
        {
            var stats = await _context.Events
                .Where(e => e.OrganizerId == organizerId)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Capacity,
                    e.Price,
                    // Contamos reservas validas
                    ReservationCount = e.Reservations != null
                        ? e.Reservations.Count(r => r.Status != ReservationStatus.Canceled)
                        : 0
                })
                .ToListAsync();

            // Calculamos porcentajes en memoria
            return stats.Select(s => new EventStatsDto
            {
                EventId = s.Id,
                EventTitle = s.Title,
                TotalReservations = s.ReservationCount,
                Revenue = s.ReservationCount * s.Price,
                OccupancyRate = s.Capacity > 0 ? (decimal)s.ReservationCount / s.Capacity * 100 : 0
            }).ToList();
        }
    }
}