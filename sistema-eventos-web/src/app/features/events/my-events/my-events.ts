import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EventService } from '../../../services/event.service';
import { Event } from '../../../core/models/event.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-my-events',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-events.html',
  styleUrl: './my-events.css'
})
export class MyEventsComponent implements OnInit {
  private eventService = inject(EventService);
  private cd = inject(ChangeDetectorRef);

  events: Event[] = [];
  isLoading = true;

  ngOnInit(): void {
    this.loadMyEvents();
  }

  loadMyEvents() {
    this.eventService.getMyEvents().subscribe({
      next: (data) => {
        this.events = data;
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

  deleteEvent(id: string, eventTitle: string) {
    Swal.fire({
      title: '¿Eliminar Evento?',
      text: `Vas a eliminar "${eventTitle}". Si tiene reservas activas no podrás hacerlo.`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      confirmButtonText: 'Sí, eliminar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.eventService.deleteEvent(id).subscribe({
          next: () => {
            Swal.fire('Eliminado', 'El evento ha sido eliminado.', 'success');
            this.loadMyEvents(); // Recargar lista
          },
          error: (err) => {
            // Aquí mostramos el mensaje de error del backend (ej. "Tiene reservas activas")
            Swal.fire('Error', err.error.message || 'No se pudo eliminar el evento.', 'error');
          }
        });
      }
    });
  }
}