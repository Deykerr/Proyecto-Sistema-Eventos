import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EventService } from '../../../services/event.service';
import { Event } from '../../../core/models/event.model';
import Swal from 'sweetalert2';
import { UpdateEventRequest } from '../../../core/models/event.model';

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

  cancelEvent(event: any) {
    Swal.fire({
      title: '¿Cancelar Evento?',
      text: `Se enviará una notificación a todos los asistentes. Esta acción no se puede deshacer fácilmente.`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      confirmButtonText: 'Sí, cancelar evento'
    }).then((result) => {
      if (result.isConfirmed) {
        
        // Construimos el objeto de actualización solo cambiando el estado
        // Nota: Necesitamos enviar todos los datos obligatorios del DTO para que el backend no se queje,
        // o idealmente tener un endpoint PATCH solo para status.
        // Como usamos PUT, rellenamos con los datos actuales.
        
        const updateRequest: UpdateEventRequest = {
          title: event.title,
          description: event.description, // Ojo: en la lista 'event' tiene description? Si no, habría que hacer un GET by ID primero.
          // Si tu lista 'getMyEvents' devuelve DTOs completos, úsalos.
          // Si faltan datos, lo correcto es hacer un getById antes de actualizar.
          
          // SOLUCIÓN RÁPIDA: Vamos a asumir que necesitas ir a "Editar" para cancelar O 
          // hacemos un fetch rápido aquí.
          
          // Mapeo seguro (Asumiendo que tienes los datos o enviamos defaults seguros si el backend valida)
          // Para evitar errores de validación, lo mejor es usar el endpoint de Editar visualmente
          // PERO para mejor UX, haremos la llamada GetById -> Update aquí mismo.
          
          categoryId: event.categoryId, // Asegúrate de que tu DTO de lista traiga esto
          startDate: event.startDate,
          endDate: event.endDate,
          location: event.location,
          latitude: event.latitude,
          longitude: event.longitude,
          capacity: event.capacity,
          price: event.price,
          isPublic: event.isPublic,
          
          status: 2 // 2 = Canceled (Según tu Enum en C#)
        };

        // Si te faltan datos en la tabla (ej. description), el PUT fallará.
        // Lo más robusto: Ir al servicio, traer el full, cambiar status, guardar.
        this.eventService.getEventById(event.id).subscribe(fullEvent => {
           const fullRequest: UpdateEventRequest = {
             title: fullEvent.title,
             description: fullEvent.description,
             categoryId: (fullEvent as any).categoryId, // Ojo con el ID
             startDate: fullEvent.startDate,
             endDate: fullEvent.endDate,
             location: fullEvent.location,
             latitude: fullEvent.latitude,
             longitude: fullEvent.longitude,
             capacity: fullEvent.capacity,
             price: fullEvent.price,
             isPublic: fullEvent.isPublic,
             status: 2 // CANCELADO
           };

           this.eventService.updateEvent(event.id, fullRequest).subscribe({
             next: () => {
               Swal.fire('Cancelado', 'El evento ha sido cancelado y los usuarios notificados.', 'success');
               this.loadMyEvents();
             },
             error: () => Swal.fire('Error', 'No se pudo cancelar.', 'error')
           });
        });
      }
    });
  }
  
}