import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import Swal from 'sweetalert2';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css'
})
export class ForgotPasswordComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  form: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  isLoading = false;

  onSubmit() {
    if (this.form.invalid) return;

    this.isLoading = true;
    const email = this.form.value.email;

    this.authService.forgotPassword({ email }).subscribe({
      next: (res) => {
        this.isLoading = false;
        // Mensaje instructivo porque estamos en desarrollo
        Swal.fire({
          icon: 'success',
          title: 'Correo enviado',
          text: 'Revisa la CONSOLA de tu Backend (.NET) para ver el token simulado.',
          confirmButtonText: 'Ir a ingresar Token'
        }).then(() => {
          this.router.navigate(['/reset-password']);
        });
      },
      error: (err) => {
        this.isLoading = false;
        Swal.fire('Error', err.error.message || 'No se pudo procesar la solicitud', 'error');
      }
    });
  }
}