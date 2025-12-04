using Sistema_Eventos.DTOs;
using Sistema_Eventos.Models;
using Sistema_Eventos.Repositories.Interfaces;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository; // Para validar si usuario existe, opcional
        private readonly IGeoService _geoService;

        public EventService(IEventRepository eventRepository, IUserRepository userRepository, IGeoService geoService)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _geoService = geoService;
        }

        public async Task<List<EventResponseDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return events.Select(MapToDto).ToList();
        }

        public async Task<EventResponseDto?> GetEventByIdAsync(Guid id)
        {
            var evento = await _eventRepository.GetEventByIdAsync(id);
            if (evento == null) return null;
            return MapToDto(evento);
        }

        public async Task<List<EventResponseDto>> GetMyEventsAsync(Guid organizerId)
        {
            var events = await _eventRepository.GetEventsByOrganizerAsync(organizerId);
            return events.Select(MapToDto).ToList();
        }

        public async Task<EventResponseDto> CreateEventAsync(CreateEventDto dto, Guid organizerId)
        {
            // Validaciones de negocio
            if (dto.EndDate < dto.StartDate)
                throw new ArgumentException("La fecha de fin no puede ser anterior a la de inicio.");

            // INTEGRACIÓN GEO: Si no mandan coordenadas, las buscamos por la dirección
            if (dto.Latitude == 0 && dto.Longitude == 0 && !string.IsNullOrEmpty(dto.Location))
            {
                var coords = await _geoService.GetCoordinatesAsync(dto.Location);
                if (coords != null)
                {
                    // Parseamos con cultura invariante (puntos vs comas)
                    if (decimal.TryParse(coords.Lat, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal lat))
                        dto.Latitude = lat;

                    if (decimal.TryParse(coords.Lon, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal lon))
                        dto.Longitude = lon;
                }
            }

            var newEvent = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId, // Asumimos que el ID existe, idealmente validar
                OrganizerId = organizerId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Location = dto.Location,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Capacity = dto.Capacity,
                AvailableSlots = dto.Capacity, // Al inicio, cupos = capacidad
                Price = dto.Price,
                IsPublic = dto.IsPublic,
                Status = EventStatus.Draft // Por defecto borrador
            };

            await _eventRepository.AddEventAsync(newEvent);

            // Recargamos para traer datos de relaciones (si fuera necesario) o mapeamos manual
            // Para simplificar retornamos el DTO básico, pero en prod haríamos GetById
            return await GetEventByIdAsync(newEvent.Id) ?? MapToDto(newEvent);
        }

        public async Task<EventResponseDto?> UpdateEventAsync(Guid id, UpdateEventDto dto, Guid userId, bool isAdmin)
        {
            var evento = await _eventRepository.GetEventByIdAsync(id);
            if (evento == null) return null;

            // Validación: Solo el dueño o Admin puede editar
            if (evento.OrganizerId != userId && !isAdmin)
                throw new UnauthorizedAccessException("No tienes permiso para modificar este evento.");

            // Actualizar campos
            evento.Title = dto.Title;
            evento.Description = dto.Description;
            evento.CategoryId = dto.CategoryId;
            evento.StartDate = dto.StartDate;
            evento.EndDate = dto.EndDate;
            evento.Location = dto.Location;
            evento.Latitude = dto.Latitude;
            evento.Longitude = dto.Longitude;
            evento.Price = dto.Price;
            evento.IsPublic = dto.IsPublic;
            evento.Status = dto.Status;

            // Nota: Cambiar capacidad es delicado si ya hay reservas, aquí lo permitimos simple
            if (dto.Capacity != evento.Capacity)
            {
                int difference = dto.Capacity - evento.Capacity;
                evento.Capacity = dto.Capacity;
                evento.AvailableSlots += difference;
            }

            evento.UpdatedAt = DateTime.UtcNow;

            await _eventRepository.UpdateEventAsync(evento);
            return MapToDto(evento);
        }

        public async Task<bool> DeleteEventAsync(Guid id, Guid userId, bool isAdmin)
        {
            var evento = await _eventRepository.GetEventByIdAsync(id);
            if (evento == null) return false;

            if (evento.OrganizerId != userId && !isAdmin)
                throw new UnauthorizedAccessException("No tienes permiso para eliminar este evento.");

            await _eventRepository.DeleteEventAsync(evento);
            return true;
        }

        // Método auxiliar para transformar Entidad -> DTO
        private static EventResponseDto MapToDto(Event evt)
        {
            return new EventResponseDto
            {
                Id = evt.Id,
                Title = evt.Title,
                Description = evt.Description,
                StartDate = evt.StartDate,
                EndDate = evt.EndDate,
                Location = evt.Location,
                Capacity = evt.Capacity,
                AvailableSlots = evt.AvailableSlots,
                Price = evt.Price,
                IsPublic = evt.IsPublic,
                Status = evt.Status.ToString(),
                OrganizerName = evt.Organizer != null ? $"{evt.Organizer.FirstName} {evt.Organizer.LastName}" : "Desconocido",
                CategoryName = evt.Category != null ? evt.Category.Name : "Sin Categoría"
            };
        }
    }
}