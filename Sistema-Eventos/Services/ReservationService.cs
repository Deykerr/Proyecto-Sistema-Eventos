using Sistema_Eventos.DTOs;
using Sistema_Eventos.Models;
using Sistema_Eventos.Repositories.Interfaces;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly INotificationService _notificationService;

        public ReservationService(IReservationRepository reservationRepository, IEventRepository eventRepository, INotificationService notificationService)
        {
            _reservationRepository = reservationRepository;
            _eventRepository = eventRepository;
            _notificationService = notificationService;
        }

        public async Task<ReservationResponseDto> CreateReservationAsync(Guid userId, CreateReservationDto dto)
        {
            // 1. Validar si el usuario YA tiene una reserva activa para este evento (RF03)
            bool alreadyReserved = await _reservationRepository.HasUserReservedEventAsync(userId, dto.EventId);
            if (alreadyReserved)
            {
                throw new InvalidOperationException("Ya tienes una reserva activa para este evento.");
            }

            // 2. Validar Evento
            var evento = await _eventRepository.GetEventByIdAsync(dto.EventId);
            if (evento == null)
                throw new Exception("El evento no existe.");

            if (evento.Status != EventStatus.Published)
                throw new Exception("No se puede reservar un evento que no está publicado.");

            // 3. Validar Cupos
            if (evento.AvailableSlots <= 0)
                throw new Exception("No hay cupos disponibles para este evento.");

            // 4. Crear Reserva
            var reservation = new Reservation
            {
                UserId = userId,
                EventId = dto.EventId,
                ReservationDate = DateTime.UtcNow,
                Status = ReservationStatus.Pending,
                TotalAmount = evento.Price
            };

            // 5. Actualizar Cupos del Evento
            evento.AvailableSlots -= 1;
            await _eventRepository.UpdateEventAsync(evento);

            // 6. Guardar Reserva
            await _reservationRepository.AddAsync(reservation);

            // 7. Notificar
            await _notificationService.SendNotificationAsync(
                userId,
                "Confirmación de Reserva",
                $"Tu reserva para '{evento.Title}' ha sido creada. Estado: {reservation.Status}",
                NotificationType.Email
            );

            return MapToDto(reservation, evento.Title);
        }

        public async Task<List<ReservationResponseDto>> GetMyReservationsAsync(Guid userId)
        {
            var reservations = await _reservationRepository.GetByUserIdAsync(userId);
            return reservations.Select(r => MapToDto(r, r.Event?.Title ?? "Desconocido")).ToList();
        }

        public async Task<List<ReservationResponseDto>> GetReservationsByEventAsync(Guid eventId, Guid userIdOfRequest)
        {
            var evento = await _eventRepository.GetEventByIdAsync(eventId);
            if (evento == null) throw new Exception("Evento no encontrado.");

            // Validación: Solo el organizador o admin puede ver la lista de asistentes
            // Nota: Aquí simplificamos asumiendo que el controlador pasa el ID correcto, 
            // idealmente verificaríamos roles aquí también o en el controlador.

            var reservations = await _reservationRepository.GetByEventIdAsync(eventId);
            return reservations.Select(r => MapToDto(r, r.Event?.Title ?? "Desconocido")).ToList();
        }

        public async Task<bool> CancelReservationAsync(Guid reservationId, Guid userId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null) return false;

            // Solo el dueño de la reserva puede cancelar (o admin, lógica extra)
            if (reservation.UserId != userId)
                throw new UnauthorizedAccessException("No puedes cancelar una reserva que no es tuya.");

            if (reservation.Status == ReservationStatus.Canceled)
                throw new Exception("La reserva ya está cancelada.");

            // Cambiar estado
            reservation.Status = ReservationStatus.Canceled;

            // Devolver el cupo al evento
            if (reservation.Event != null)
            {
                reservation.Event.AvailableSlots += 1;
                await _eventRepository.UpdateEventAsync(reservation.Event);
            }

            await _reservationRepository.UpdateAsync(reservation);
            return true;
        }

        private static ReservationResponseDto MapToDto(Reservation r, string eventTitle)
        {
            return new ReservationResponseDto
            {
                Id = r.Id,
                EventId = r.EventId,
                EventTitle = eventTitle,
                UserName = r.User != null ? $"{r.User.FirstName} {r.User.LastName}" : "Usuario",
                ReservationDate = r.ReservationDate,
                Status = r.Status.ToString(),
                TotalAmount = r.TotalAmount
            };
        }
    }
}