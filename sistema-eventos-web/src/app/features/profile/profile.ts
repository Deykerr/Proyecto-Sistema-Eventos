import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service'; // Para logout si se requiere
import { UserProfile } from '../../core/models/user.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class ProfileComponent implements OnInit {
  private userService = inject(UserService);
  private authService = inject(AuthService); // Opcional
  private fb = inject(FormBuilder);
  private cd = inject(ChangeDetectorRef);

  profile: UserProfile | null = null;
  isLoading = true;
  isEditing = false; // Controla si mostramos el formulario o solo lectura

  // Formulario para editar
  form: FormGroup = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required]
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile() {
    this.userService.getProfile().subscribe({
      next: (data) => {
        this.profile = data;
        // Cargamos los datos actuales en el formulario
        this.form.patchValue({
          firstName: data.firstName,
          lastName: data.lastName
        });
        this.isLoading = false;
        this.cd.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
        this.cd.detectChanges();
      }
    });
  }

  toggleEdit() {
    this.isEditing = !this.isEditing;
    // Si cancela edición, reseteamos el form a los datos originales
    if (!this.isEditing && this.profile) {
      this.form.patchValue({
        firstName: this.profile.firstName,
        lastName: this.profile.lastName
      });
    }
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.isLoading = true;
    this.userService.updateProfile(this.form.value).subscribe({
      next: (updatedProfile) => {
        this.profile = updatedProfile;
        this.isEditing = false;
        this.isLoading = false;
        Swal.fire('Actualizado', 'Tu perfil ha sido modificado correctamente.', 'success');
        this.cd.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        Swal.fire('Error', 'No se pudieron guardar los cambios.', 'error');
        this.cd.detectChanges();
      }
    });
  }
}