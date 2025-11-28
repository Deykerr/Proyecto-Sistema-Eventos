using Sistema_Eventos.DTOs;
using Sistema_Eventos.Models; // O Entities

namespace Sistema_Eventos.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventResponseDto>> GetAllEventsAsync();
        Task<EventResponseDto?> GetEventByIdAsync(Guid id);
        Task<List<EventResponseDto>> GetMyEventsAsync(Guid organizerId);

        // Devuelve el evento creado
        Task<EventResponseDto> CreateEventAsync(CreateEventDto createDto, Guid organizerId);

        // Devuelve null si no encuentra el evento o false si falla validación
        Task<EventResponseDto?> UpdateEventAsync(Guid id, UpdateEventDto updateDto, Guid userId, bool isAdmin);

        // Devuelve true si borró, false si no encontró o no tiene permiso
        Task<bool> DeleteEventAsync(Guid id, Guid userId, bool isAdmin);
    }
}