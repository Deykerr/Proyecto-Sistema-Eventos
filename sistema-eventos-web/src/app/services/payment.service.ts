import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { PaymentIntentRequest, PaymentResponse } from '../core/models/external.model'; // Asegúrate de que estén aquí

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  
  // Puerto 7245
  private apiUrl = 'https://localhost:7245/api/v1/payments';

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }

  // 1. Crear intención de pago (Obtener URL simulada)
  createPaymentIntent(data: PaymentIntentRequest): Observable<PaymentResponse> {
    return this.http.post<PaymentResponse>(`${this.apiUrl}/create-intent`, data, { headers: this.getHeaders() });
  }

  // 2. Simular Webhook (Para que la reserva pase a CONFIRMED sin usar Postman)
  // Este endpoint es público en tu controller, no requiere headers de auth
  simulateWebhook(reservationId: string, transactionId: string): Observable<any> {
    const payload = {
      eventType: 'payment.succeeded',
      transactionId: transactionId,
      reservationId: reservationId
    };
    return this.http.post(`${this.apiUrl}/webhook`, payload);
  }
}