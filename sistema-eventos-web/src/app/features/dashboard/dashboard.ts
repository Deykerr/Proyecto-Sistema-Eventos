import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core'; // <--- Importar ChangeDetectorRef
import { CommonModule } from '@angular/common';
import { ReportService } from '../../services/report.service';
import { DashboardStats, EventStats } from '../../core/models/report.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent implements OnInit {
  private reportService = inject(ReportService);
  private cd = inject(ChangeDetectorRef); // <--- Inyectar el detector

  stats: DashboardStats | null = null;
  eventStats: EventStats[] = [];
  isLoading = true;

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    // 1. Cargar KPIs
    this.reportService.getDashboardStats().subscribe({
      next: (data) => {
        this.stats = data;
        // No desactivamos isLoading aquí todavía, esperamos a la tabla
        this.cd.detectChanges(); // <--- Forzar actualización
      },
      error: (err) => {
        console.error(err);
        // Si falla uno, al menos mostramos lo que haya o quitamos el loading
        this.cd.detectChanges();
      }
    });

    // 2. Cargar Tabla
    this.reportService.getEventsStats().subscribe({
      next: (data) => {
        this.eventStats = data;
        this.isLoading = false; // Aquí sí quitamos el spinner
        this.cd.detectChanges(); // <--- Forzar actualización
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
        this.cd.detectChanges(); // <--- Forzar actualización
      }
    });
  }

  // --- FUNCIÓN DE DESCARGA ---
  downloadReport() {
    Swal.fire({
      title: 'Generando Reporte...',
      text: 'Por favor espera un momento.',
      allowOutsideClick: false,
      didOpen: () => {
        Swal.showLoading();
      }
    });

    this.reportService.exportReport().subscribe({
      next: (blob) => {
        Swal.close(); // Cerramos el loading

        // Truco para descargar el Blob desde el navegador
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        // Nombre del archivo sugerido
        a.download = `Reporte_Eventos_${new Date().toISOString().slice(0,10)}.csv`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error(err);
        Swal.fire('Error', 'No se pudo descargar el reporte.', 'error');
      }
    });
  }
}