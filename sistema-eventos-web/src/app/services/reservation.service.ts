import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { CreateReservationRequest, Reservation } from '../core/models/reservation.model';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  
  // Recuerda usar el puerto correcto (7245)
  private apiUrl = 'https://localhost:7245/api/v1/reservations';

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  // 1. Crear Reserva
  createReservation(data: CreateReservationRequest): Observable<Reservation> {
    return this.http.post<Reservation>(this.apiUrl, data, { headers: this.getHeaders() });
  }

  // 2. Obtener Mis Reservas
  getMyReservations(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(`${this.apiUrl}/my-reservations`, { headers: this.getHeaders() });
  }

  // 3. Cancelar Reserva
  cancelReservation(id: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/cancel`, {}, { headers: this.getHeaders() });
  }
}