using Sistema_Eventos.DTOs;

namespace Sistema_Eventos.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherResponseDto?> GetCurrentWeatherAsync(decimal lat, decimal lon);
    }
}