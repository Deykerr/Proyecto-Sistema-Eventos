using Sistema_Eventos.DTOs;

namespace Sistema_Eventos.Services.Interfaces
{
    public interface IGeoService
    {
        // Devuelve (Lat, Lon) dado una dirección
        Task<GeoResponseDto?> GetCoordinatesAsync(string address);
    }
}