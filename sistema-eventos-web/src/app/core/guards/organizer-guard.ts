import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import Swal from 'sweetalert2';

export const organizerGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const role = authService.getUserRole();

  // Permitimos acceso a Admin u Organizador
  if (authService.isAuthenticated() && (role === 'Organizer' || role === 'Admin')) {
    return true;
  }

  Swal.fire('Acceso Restringido', 'Esta sección es para Organizadores.', 'warning');
  router.navigate(['/events']);
  return false;
};