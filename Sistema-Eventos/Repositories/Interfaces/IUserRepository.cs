using Sistema_Eventos.Models;

namespace Sistema_Eventos.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // Métodos de lectura
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email); // Crucial para el Login

        // Métodos de escritura
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user); // Opcional, por si se requiere borrado físico
    }
}