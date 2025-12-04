using System.Text.Json;
using Sistema_Eventos.DTOs;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<WeatherResponseDto?> GetCurrentWeatherAsync(decimal lat, decimal lon)
        {
            var apiKey = _configuration["OpenWeather:ApiKey"]; // Configurar en appsettings
            if (string.IsNullOrEmpty(apiKey)) return null;

            // Usamos InvariantCulture para que los decimales usen punto (13.5) y no coma (13,5) en la URL
            var latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lonStr = lon.ToString(System.Globalization.CultureInfo.InvariantCulture);

            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={latStr}&lon={lonStr}&appid={apiKey}&units=metric&lang=es";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            return JsonSerializer.Deserialize<WeatherResponseDto>(content, options);
        }
    }
}