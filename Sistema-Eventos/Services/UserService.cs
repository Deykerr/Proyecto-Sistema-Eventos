using Sistema_Eventos.DTOs;
using Sistema_Eventos.Repositories.Interfaces;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfileDto?> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserProfileDto?> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            // Actualizamos solo los campos permitidos
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user);

            // Devolvemos el perfil actualizado
            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<List<UserProfileDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(u => new UserProfileDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role.ToString(),
                IsActive = u.IsActive, // <--- Importante
                CreatedAt = u.CreatedAt
            }).ToList();
        }

        public async Task<UserProfileDto?> GetUserByIdAdminAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> ToggleUserStatusAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return false;

            // Invertimos el estado (Si es true pasa a false, y viceversa)
            user.IsActive = !user.IsActive;

            await _userRepository.UpdateUserAsync(user);
            return true;
        }
    }
}