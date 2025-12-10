using Sistema_Eventos.DTOs;
using Sistema_Eventos.Models; // Para acceder a User si es necesario devolverlo

namespace Sistema_Eventos.Services.Interfaces
{
    public interface IAuthService
    {
        // Devuelve el User creado o null si falla
        Task<User> RegisterAsync(RegisterDto request);

        // Devuelve el objeto con el Token o null si falla
        Task<AuthResponseDto?> LoginAsync(LoginDto request);

        Task ForgotPasswordAsync(ForgotPasswordDto request); // Retorna un mensaje o el token simulado
        Task<bool> ResetPasswordAsync(ResetPasswordDto request);
    }
}