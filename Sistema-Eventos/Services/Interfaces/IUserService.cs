using Sistema_Eventos.DTOs;

namespace Sistema_Eventos.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetProfileAsync(Guid userId);
        Task<UserProfileDto?> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);

        Task<List<UserProfileDto>> GetAllUsersAsync(); // Ver todos
        Task<UserProfileDto?> GetUserByIdAdminAsync(Guid id); // Ver detalle de otro usuario
        Task<bool> ToggleUserStatusAsync(Guid id); // Banear/Activar
    }
}