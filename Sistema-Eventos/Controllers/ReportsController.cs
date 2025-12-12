using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Eventos.Services.Interfaces;

namespace Sistema_Eventos.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    [Authorize(Roles = "Organizer,Admin")] // Bloqueado para usuarios normales
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: api/v1/reports/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var stats = await _reportService.GetDashboardStatsAsync(userId);
            return Ok(stats);
        }

        // GET: api/v1/reports/events-stats
        [HttpGet("events-stats")]
        public async Task<IActionResult> GetEventsStats()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var stats = await _reportService.GetEventsStatsAsync(userId);
            return Ok(stats);
        }

        // GET: api/v1/reports/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportReport()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var csvBytes = await _reportService.ExportStatsToCsvAsync(userId);
            var fileName = $"Reporte_Eventos_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";

            return File(csvBytes, "text/csv", fileName);
        }
    }
}