import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EventService } from '../../../services/event.service';
import { WeatherService } from '../../../services/weather.service';
import { AuthService } from '../../../services/auth.service';
import { Event } from '../../../core/models/event.model';
import { WeatherResponse } from '../../../core/models/external.model';
import { ReservationService } from '../../../services/reservation.service';
import { CreateReservationRequest } from '../../../core/models/reservation.model';

import Swal from 'sweetalert2';

@Component({
  selector: 'app-event-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './event-detail.html',
  styleUrl: './event-detail.css'
})
export class EventDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private eventService = inject(EventService);
  private weatherService = inject(WeatherService);
  public authService = inject(AuthService); // Público para usar en HTML
  private cd = inject(ChangeDetectorRef);
  private reservationService = inject(ReservationService);

  event: Event | null = null;
  weather: WeatherResponse | null = null;
  isLoading = true;

  // Mapa visual para Google Maps
  mapUrl: string = '';

  ngOnInit(): void {
    // Obtener el ID de la URL (ej: /events/guid-del-evento)
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadEvent(id);
    } else {
      this.router.navigate(['/events']);
    }
  }

  loadEvent(id: string) {
    this.eventService.getEventById(id).subscribe({
      next: (data) => {
        this.event = data;
        // Generar link de Google Maps con las coordenadas del backend [cite: 115]
        this.mapUrl = `https://www.google.com/maps/search/?api=1&query=${data.latitude},${data.longitude}`;

        // Una vez cargado el evento, cargamos el clima
        this.loadWeather(id);
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
        Swal.fire('Error', 'No se pudo cargar el evento', 'error');
        this.router.navigate(['/events']);
      }
    });
  }

  loadWeather(id: string) {
    this.weatherService.getEventWeather(id).subscribe({
      next: (data) => {
        this.weather = data;
        this.isLoading = false;
        this.cd.detectChanges(); // Forzar actualización visual
      },
      error: (err) => {
        console.warn('No se pudo cargar el clima', err);
        this.isLoading = false;
        this.cd.detectChanges();
      }
    });
  }

  onReserve() {
    if (!this.authService.isAuthenticated()) {
      Swal.fire('Acceso Requerido', 'Debes iniciar sesión para reservar', 'info')
        .then(() => this.router.navigate(['/login']));
      return;
    }

    // Aquí implementaremos la reserva en el siguiente paso
    Swal.fire({
      title: '¿Confirmar Reserva?',
      text: `Estás a punto de reservar para: ${this.event?.title}`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Sí, reservar',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed && this.event) {
        this.processReservation();
      }
    });
  }

  processReservation() {
    this.isLoading = true;
    const request: CreateReservationRequest = {
      eventId: this.event!.id
    };

    this.reservationService.createReservation(request).subscribe({
      next: () => {
        this.isLoading = false;
        Swal.fire({
          title: '¡Reserva Exitosa!',
          text: 'Tu reserva ha sido registrada. Puedes verla en "Mis Reservas".',
          icon: 'success',
          confirmButtonText: 'Ir a Mis Reservas'
        }).then(() => {
          this.router.navigate(['/my-reservations']);
        });
      },
      error: (err) => {
        this.isLoading = false;
        console.error(err);
        Swal.fire('Error', err.error.message || 'No se pudo procesar la reserva', 'error');
      }
    });

  }

  publish() {
  Swal.fire({
    title: '¿Publicar Evento?',
    text: "Una vez publicado, los usuarios podrán hacer reservas.",
    icon: 'info',
    showCancelButton: true,
    confirmButtonText: 'Sí, publicar',
    confirmButtonColor: '#28a745'
  }).then((result) => {
    if (result.isConfirmed && this.event) {
      this.eventService.publishEvent(this.event.id).subscribe({
        next: () => {
          Swal.fire('Publicado', 'El evento ya es público.', 'success');
          // Recargamos el evento para que se actualice el estado en pantalla
          this.loadEvent(this.event!.id);
        },
        error: (err) => {
          console.error(err);
          Swal.fire('Error', 'No tienes permiso o hubo un fallo.', 'error');
        }
      });
    }
  });
}
}