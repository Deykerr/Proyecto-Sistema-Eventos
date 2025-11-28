using Sistema_Eventos.DTOs;

namespace Sistema_Eventos.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationResponseDto> CreateReservationAsync(Guid userId, CreateReservationDto dto);
        Task<List<ReservationResponseDto>> GetMyReservationsAsync(Guid userId);
        Task<List<ReservationResponseDto>> GetReservationsByEventAsync(Guid eventId, Guid userIdOfRequest);
        Task<bool> CancelReservationAsync(Guid reservationId, Guid userId);
    }
}