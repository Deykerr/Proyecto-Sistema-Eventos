import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import Swal from 'sweetalert2'; // Importamos SweetAlert2 para alertas bonitas

// Importamos nuestro servicio y la interfaz
import { AuthService } from '../../../services/auth.service';
import { LoginRequest } from '../../../core/models/auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  // Importamos Módulos necesarios para este componente
  imports: [CommonModule, ReactiveFormsModule, RouterLink], 
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  // Inyecciones de dependencias (Modo moderno con 'inject')
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  // Definición del formulario y sus validaciones
  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  // Variable para mostrar loading en el botón
  isLoading = false;

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched(); // Marca campos en rojo si hay error
      return;
    }

    this.isLoading = true;
    const credentials: LoginRequest = this.loginForm.value;

    this.authService.login(credentials).subscribe({
      next: (response) => {
        // 1. Éxito: Guardamos token (lo hace el service) y redirigimos
        this.isLoading = false;
        
        Swal.fire({
          icon: 'success',
          title: '¡Bienvenido!',
          text: `Hola de nuevo, ${response.email}`,
          timer: 2000,
          showConfirmButton: false
        });

        // Redirigir según el rol (opcional) o al home
        // Por ahora lo mandamos a una ruta raíz o dashboard
        this.router.navigate(['/events']); 
      },
      error: (err) => {
        // 2. Error: Mostramos mensaje
        this.isLoading = false;
        console.error('Login error:', err);
        
        Swal.fire({
          icon: 'error',
          title: 'Error de acceso',
          text: err.error.message || 'Credenciales incorrectas o servidor no disponible'
        });
      }
    });
  }
}