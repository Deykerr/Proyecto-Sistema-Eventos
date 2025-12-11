import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { CreateEventRequest, Event } from '../core/models/event.model'; // Tu interfaz


@Injectable({
  providedIn: 'root'
})
export class EventService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  
  // Ajusta el puerto a tu backend (ej. 7152 o 7000)
  private apiUrl = 'https://localhost:7245/api/v1/events';

  // Helper para enviar el Token en el Header
  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  // 1. Obtener todos los eventos (Público o Privado según tu backend)
  getAllEvents(): Observable<Event[]> {
    // Si tu endpoint GET /api/v1/events es público, no necesitas headers.
    // Si requiere login, agrega { headers: this.getHeaders() }
    return this.http.get<Event[]>(this.apiUrl);
  }

  // 2. Obtener un evento por ID
  getEventById(id: string): Observable<Event> {
    return this.http.get<Event>(`${this.apiUrl}/${id}`);
  }

  // 3. Crear evento (POST)
  createEvent(eventData: CreateEventRequest): Observable<Event> {
    // Aquí sí necesitamos enviar token porque el endpoint es seguro [Authorize]
    return this.http.post<Event>(this.apiUrl, eventData, { headers: this.getHeaders() });
  }

  // 4. Publicar Evento
publishEvent(id: string): Observable<any> {
  return this.http.post(`${this.apiUrl}/${id}/publish`, {}, { headers: this.getHeaders() });
}

  // Aquí agregaremos crear, editar y eliminar más adelante
}