import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { AppNotification } from '../core/models/notification.model'; // Modelo creado anteriormente

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  // Puerto 7245
  private apiUrl = 'https://localhost:7245/api/v1/notifications';

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }

  getUserNotifications(): Observable<AppNotification[]> {
    return this.http.get<AppNotification[]>(this.apiUrl, { headers: this.getHeaders() });
  }
}