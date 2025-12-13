import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../../services/auth.service'; // Ajusta la ruta si es necesario
import { catchError, switchMap, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // 1. Obtener el token actual
  const token = authService.getToken();

  // 2. Clonar la petición y agregar el Token en el Header (si existe)
  // Esto nos ahorra tener que poner { headers: ... } en cada servicio manualmente
  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  // 3. Pasar la petición al siguiente manejador y escuchar errores
  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      
      // Si el error es 401 (Unauthorized) y NO es una petición de login/refresh
      if (error.status === 401 && !authReq.url.includes('/auth/login') && !authReq.url.includes('/auth/refresh')) {
        
        // Intentamos refrescar el token
        return authService.refreshToken().pipe(
          switchMap((response) => {
            // ÉXITO: Tenemos nuevo token
            // Clonamos la petición original que falló, pero con el NUEVO token
            const newReq = req.clone({
              setHeaders: { Authorization: `Bearer ${response.token}` }
            });
            
            // Reintentamos la petición original
            return next(newReq);
          }),
          catchError((refreshError) => {
            // FALLO: El refresh token también expiró o es inválido.
            // No hay nada más que hacer, cerramos sesión.
            authService.logout();
            router.navigate(['/login']);
            return throwError(() => refreshError);
          })
        );
      }

      // Si es otro error (ej. 400, 500), lo dejamos pasar
      return throwError(() => error);
    })
  );
};