import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode'; // npm install jwt-decode

// Importamos tus modelos definidos anteriormente
import { 
  AuthResponse, 
  LoginRequest, 
  RegisterRequest, 
  ForgotPasswordRequest, 
  ResetPasswordRequest 
} from '../core/models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  
  // AJUSTA EL PUERTO AQUÍ: Pon el puerto donde corre tu Backend .NET (ej. 7000, 5024, etc.)
  private apiUrl = 'https://localhost:7245/api/v1/auth'; 

  // State Management: Para saber en tiempo real si el usuario está logueado
  // BehaviorSubject guarda el valor actual y lo emite a quien se suscriba
  private currentUserTokenSubject = new BehaviorSubject<string | null>(localStorage.getItem('token'));
  public userToken$ = this.currentUserTokenSubject.asObservable();

  constructor() { }

  // --- 1. LOGIN ---
  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        if (response.token) {
          // Guardamos el token en el navegador
          localStorage.setItem('token', response.token);
          // Notificamos a toda la app que hay un nuevo usuario
          this.currentUserTokenSubject.next(response.token);
        }
      })
    );
  }

  // --- 2. REGISTER ---
  register(data: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data);
  }

  // --- 3. LOGOUT ---
  logout(): void {
    localStorage.removeItem('token');
    this.currentUserTokenSubject.next(null);
    // Aquí podrías redirigir al login: this.router.navigate(['/auth/login']);
  }

  // --- 4. RECUPERAR CONTRASEÑA ---
  forgotPassword(data: ForgotPasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgot-password`, data);
  }

  resetPassword(data: ResetPasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, data);
  }

  // --- MÉTODOS AUXILIARES ---

  // Obtener el token actual (sin observable)
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  // Verificar si está logueado
  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    // Opcional: Verificar expiración
    try {
      const decoded: any = jwtDecode(token);
      const currentTime = Date.now() / 1000;
      if (decoded.exp < currentTime) {
        this.logout(); // Si expiró, cerramos sesión
        return false;
      }
      return true;
    } catch (error) {
      return false;
    }
  }

  // Obtener Rol del usuario desde el Token
  getUserRole(): string {
    const token = this.getToken();
    if (!token) return '';

    try {
      const decoded: any = jwtDecode(token);
      // .NET a veces pone el rol en "role" o en una URL larga de schemas.microsoft...
      // Buscamos ambas opciones:
      return decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '';
    } catch (error) {
      return '';
    }
  }
  
  // Obtener ID del usuario
  getUserId(): string {
    const token = this.getToken();
    if (!token) return '';
    try {
      const decoded: any = jwtDecode(token);
      // 'nameid' es el estándar para el ID en JWT de .NET
      return decoded.nameid || decoded.sub || '';
    } catch {
      return '';
    }
  }
}