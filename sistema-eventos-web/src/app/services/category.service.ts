import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../core/models/category.model'; // Asegúrate de tener este modelo
import { AuthService } from './auth.service';
import { HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  // OJO: Usa el puerto correcto 7245
  private apiUrl = 'https://localhost:7245/api/v1/categories';

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }

  // GET (Público)
  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  // POST (Admin)
  create(category: any): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, category, { headers: this.getHeaders() });
  }

  // DELETE (Admin)
  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}