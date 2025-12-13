using Sistema_Eventos.Models;
using Sistema_Eventos.Repositories.Interfaces;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Workers
{
    public class ReservationReminderWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReservationReminderWorker> _logger;

        public ReservationReminderWorker(IServiceScopeFactory scopeFactory, ILogger<ReservationReminderWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de Recordatorios Automáticos INICIADO.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar recordatorios.");
                }

                // Esperar 1 hora antes de volver a ejecutar
                // Para pruebas puedes cambiarlo a TimeSpan.FromMinutes(1)
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task ProcessRemindersAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                // Obtenemos los servicios dentro del scope manual
                var reservationRepo = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // Lógica: Buscar eventos que empiecen entre "Mañana a esta hora" y "Mañana + 1 hora"
                // Así, al correr este script cada hora, cubrimos todos los eventos con 24h de anticipación exacta.
                var now = DateTime.UtcNow;
                var startWindow = now.AddHours(24);
                var endWindow = now.AddHours(25);

                var upcomingReservations = await reservationRepo.GetConfirmedReservationsForDateRangeAsync(startWindow, endWindow);

                if (upcomingReservations.Any())
                {
                    _logger.LogInformation($"Se encontraron {upcomingReservations.Count} reservas para recordar.");

                    foreach (var res in upcomingReservations)
                    {
                        var message = $"Hola {res.User?.FirstName}, te recordamos que tu evento '{res.Event?.Title}' comienza mañana a las {res.Event?.StartDate:HH:mm}. ¡Te esperamos!";

                        // Enviamos la notificación (Esto guardará en BD y simulará el email)
                        await notificationService.SendNotificationAsync(
                            res.UserId,
                            "Recordatorio de Evento 📅",
                            message,
                            NotificationType.Email
                        );
                    }
                }
            }
        }
    }
}