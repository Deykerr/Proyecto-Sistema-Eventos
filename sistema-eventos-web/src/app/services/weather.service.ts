import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WeatherResponse } from '../core/models/external.model'; // Asegúrate de tener esta interfaz

@Injectable({
  providedIn: 'root'
})
export class WeatherService {
  private http = inject(HttpClient);
  // Puerto 7245 (el que te funciona)
  private apiUrl = 'https://localhost:7245/api/v1/events'; 

  // GET /api/v1/events/{id}/weather
  getEventWeather(eventId: string): Observable<WeatherResponse> {
    return this.http.get<WeatherResponse>(`${this.apiUrl}/${eventId}/weather`);
  }
}