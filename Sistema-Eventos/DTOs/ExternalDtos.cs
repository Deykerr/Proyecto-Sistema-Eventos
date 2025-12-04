namespace Sistema_Eventos.DTOs
{
    // --- CLIMA ---
    public class WeatherResponseDto
    {
        public MainData Main { get; set; } = new();
        public List<WeatherDescription> Weather { get; set; } = new();
        public string Name { get; set; } = string.Empty;
    }

    public class MainData
    {
        public float Temp { get; set; }
        public float Humidity { get; set; }
    }

    public class WeatherDescription
    {
        public string Description { get; set; } = string.Empty;
        public string Main { get; set; } = string.Empty;
    }

    // --- GEOLOCALIZACIÓN ---
    public class GeoResponseDto
    {
        public string Lat { get; set; } = string.Empty;
        public string Lon { get; set; } = string.Empty;
        public string Display_Name { get; set; } = string.Empty;
    }
}