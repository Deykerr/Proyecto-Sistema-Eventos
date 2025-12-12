import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { UserProfile, UpdateProfileRequest } from '../core/models/user.model'; // Asegúrate de tener estos modelos

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  // Puerto 7245
  private apiUrl = 'https://localhost:7245/api/v1/users';

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }

  // GET: Ver mi perfil
  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/profile`, { headers: this.getHeaders() });
  }

  // PUT: Actualizar mis datos
  updateProfile(data: UpdateProfileRequest): Observable<UserProfile> {
    return this.http.put<UserProfile>(`${this.apiUrl}/profile`, data, { headers: this.getHeaders() });
  }

  // --- NUEVOS MÉTODOS PARA ADMIN ---

  // GET: Obtener todos los usuarios
  getAllUsers(): Observable<UserProfile[]> {
    return this.http.get<UserProfile[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  // PUT: Cambiar estado (Banear/Activar)
  toggleUserStatus(id: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/status`, {}, { headers: this.getHeaders() });
  }
}