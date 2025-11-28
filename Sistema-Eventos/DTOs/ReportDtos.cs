namespace Sistema_Eventos.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalEvents { get; set; }
        public int TotalReservations { get; set; }
        public decimal TotalRevenue { get; set; } // Ingresos totales
        public int ActiveEvents { get; set; }
    }

    public class EventStatsDto
    {
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public int TotalReservations { get; set; }
        public decimal OccupancyRate { get; set; } // Porcentaje de ocupación
        public decimal Revenue { get; set; } // Ganancias de este evento
    }
}