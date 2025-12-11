import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { AuthService } from '../../../services/auth.service';
import { ResetPasswordRequest } from '../../../core/models/auth.model';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css'
})
export class ResetPasswordComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  form: FormGroup = this.fb.group({
    token: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required]
  }, { validators: this.passwordMatchValidator });

  isLoading = false;

  passwordMatchValidator(form: FormGroup) {
    const pass = form.get('newPassword')?.value;
    const confirm = form.get('confirmPassword')?.value;
    return pass === confirm ? null : { mismatch: true };
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.isLoading = true;
    const req: ResetPasswordRequest = {
      token: this.form.value.token,
      newPassword: this.form.value.newPassword,
      confirmPassword: this.form.value.confirmPassword
    };

    this.authService.resetPassword(req).subscribe({
      next: () => {
        this.isLoading = false;
        Swal.fire({
          icon: 'success',
          title: 'Contraseña Actualizada',
          text: 'Ya puedes iniciar sesión con tu nueva contraseña.',
          confirmButtonText: 'Ir al Login'
        }).then(() => {
          this.router.navigate(['/login']);
        });
      },
      error: (err) => {
        this.isLoading = false;
        Swal.fire('Error', 'Token inválido o expirado.', 'error');
      }
    });
  }
}