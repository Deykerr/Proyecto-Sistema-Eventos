using Microsoft.EntityFrameworkCore;
using Sistema_Eventos.Data;
using Sistema_Eventos.Models;
using Sistema_Eventos.Repositories.Interfaces;

namespace Sistema_Eventos.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await _context.Reservations
                .Include(r => r.Event) // Incluimos datos del evento para validar reglas
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Reservation>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Event) // El usuario querrá ver el título del evento
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetByEventIdAsync(Guid eventId)
        {
            return await _context.Reservations
                .Where(r => r.EventId == eventId)
                .Include(r => r.User) // El organizador necesita ver quién reservó
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }

        public async Task<bool> HasUserReservedEventAsync(Guid userId, Guid eventId)
        {
            // Retorna true si existe alguna reserva NO cancelada para ese usuario y evento
            return await _context.Reservations
                .AnyAsync(r => r.UserId == userId
                            && r.EventId == eventId
                            && r.Status != ReservationStatus.Canceled);
        }

        public async Task AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }
    }
}