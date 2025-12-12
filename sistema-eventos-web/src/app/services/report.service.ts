import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { DashboardStats, EventStats } from '../core/models/report.model';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  // Puerto 7245
  private apiUrl = 'https://localhost:7245/api/v1/reports';

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }

  // KPIs Generales
  getDashboardStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.apiUrl}/dashboard`, { headers: this.getHeaders() });
  }

  // Tabla detallada por evento
  getEventsStats(): Observable<EventStats[]> {
    return this.http.get<EventStats[]>(`${this.apiUrl}/events-stats`, { headers: this.getHeaders() });
  }


  // --- NUEVO MÉTODO PARA DESCARGAR ---
  exportReport(): Observable<Blob> {
    // Importante: responseType: 'blob' le dice a Angular que viene un archivo binario
    return this.http.get(`${this.apiUrl}/export`, {
      headers: this.getHeaders(),
      responseType: 'blob'
    });
  }
}