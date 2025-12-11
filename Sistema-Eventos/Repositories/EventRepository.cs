using Microsoft.EntityFrameworkCore;
using Sistema_Eventos.Data;
using Sistema_Eventos.Models;
using Sistema_Eventos.Repositories.Interfaces;

namespace Sistema_Eventos.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            // --- MODIFICACIÓN: FILTRAR SOLO PUBLICADOS ---
            return await _context.Events
                .Where(e => e.Status == EventStatus.Published) // <--- Filtro Clave [cite: 142]
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .OrderBy(e => e.StartDate) // Opcional: ordenar por fecha
                .ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Event>> GetEventsByOrganizerAsync(Guid organizerId)
        {
            return await _context.Events
                .Where(e => e.OrganizerId == organizerId)
                .Include(e => e.Category)
                .ToListAsync();
        }

        public async Task AddEventAsync(Event evento)
        {
            await _context.Events.AddAsync(evento);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEventAsync(Event evento)
        {
            _context.Events.Update(evento);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEventAsync(Event evento)
        {
            _context.Events.Remove(evento);
            await _context.SaveChangesAsync();
        }
    }
}