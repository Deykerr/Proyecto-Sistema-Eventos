import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../services/user.service';
import { UserProfile } from '../../../core/models/user.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-management.html',
  styleUrl: './user-management.css'
})
export class UserManagementComponent implements OnInit {
  private userService = inject(UserService);
  private cd = inject(ChangeDetectorRef);

  users: UserProfile[] = [];
  isLoading = true;

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe({
      next: (data) => {
        this.users = data;
        this.isLoading = false;
        this.cd.detectChanges(); // Forzar actualización de vista
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
        this.cd.detectChanges();
      }
    });
  }

  toggleStatus(user: UserProfile) {
    const action = user.isActive ? 'Desactivar' : 'Activar';
    const color = user.isActive ? '#d33' : '#28a745';

    Swal.fire({
      title: `¿${action} usuario?`,
      text: `Estás a punto de cambiar el estado de ${user.email}`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: color,
      confirmButtonText: `Sí, ${action}`
    }).then((result) => {
      if (result.isConfirmed) {
        this.userService.toggleUserStatus(user.id).subscribe({
          next: () => {
            // Actualizamos la lista localmente para que se vea rápido
            user.isActive = !user.isActive;
            Swal.fire('Actualizado', `El usuario ha sido ${user.isActive ? 'activado' : 'desactivado'}.`, 'success');
            this.cd.detectChanges();
          },
          error: () => {
            Swal.fire('Error', 'No se pudo cambiar el estado.', 'error');
          }
        });
      }
    });
  }
}