using System.Text.Json;
using Sistema_Eventos.DTOs;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Services
{
    public class GeoService : IGeoService
    {
        private readonly HttpClient _httpClient;

        public GeoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // OpenStreetMap requiere un User-Agent válido
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "SistemaEventosEstudiante/1.0");
        }

        public async Task<GeoResponseDto?> GetCoordinatesAsync(string address)
        {
            var url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(address)}&limit=1";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Nominatim devuelve una lista, tomamos el primero
                var results = JsonSerializer.Deserialize<List<GeoResponseDto>>(content, options);

                return results?.FirstOrDefault();
            }
            catch
            {
                return null; // Manejo básico de errores de red
            }
        }
    }
}