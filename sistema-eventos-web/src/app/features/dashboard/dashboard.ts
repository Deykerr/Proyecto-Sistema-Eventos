import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core'; // <--- Importar ChangeDetectorRef
import { CommonModule } from '@angular/common';
import { ReportService } from '../../services/report.service';
import { DashboardStats, EventStats } from '../../core/models/report.model';

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
}