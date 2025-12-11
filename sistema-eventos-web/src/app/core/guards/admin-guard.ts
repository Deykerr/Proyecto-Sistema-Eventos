import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import Swal from 'sweetalert2';

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Verificamos si es Admin
  if (authService.isAuthenticated() && authService.getUserRole() === 'Admin') {
    return true;
  }

  // Si no es Admin, lo pateamos fuera
  Swal.fire('Acceso Denegado', 'Esta sección es solo para Administradores.', 'warning');
  router.navigate(['/events']);
  return false;
};