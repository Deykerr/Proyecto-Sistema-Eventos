import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ReservationService } from '../../../services/reservation.service';
import { Reservation } from '../../../core/models/reservation.model';
import { PaymentService } from '../../../services/payment.service';
import { PaymentIntentRequest } from '../../../core/models/external.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-my-reservations',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-reservations.html',
  styleUrl: './my-reservations.css'
})
export class MyReservationsComponent implements OnInit {
  private reservationService = inject(ReservationService);
  private cd = inject(ChangeDetectorRef);
  private paymentService = inject(PaymentService);

  reservations: Reservation[] = [];
  isLoading = true;

  ngOnInit(): void {
    this.loadReservations();
  }

  loadReservations() {
    this.reservationService.getMyReservations().subscribe({
      next: (data) => {
        this.reservations = data;
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

  // Método para Pagar
  payReservation(reservationId: string) {
    this.isLoading = true; // Mostrar spinner mientras contactamos al banco simulado
    
    const request: PaymentIntentRequest = { reservationId };

    this.paymentService.createPaymentIntent(request).subscribe({
      next: (response) => {
        this.isLoading = false;
        
        // Mostrar alerta con la "Pasarela de Pagos Simulada"
        Swal.fire({
          title: 'Pasarela de Pagos',
          text: `Se ha generado una transacción ID: ${response.transactionId}. ¿Deseas completar el pago?`,
          icon: 'info',
          showCancelButton: true,
          confirmButtonText: 'Sí, Pagar (Simular Éxito)',
          cancelButtonText: 'Cancelar',
          confirmButtonColor: '#28a745'
        }).then((result) => {
          if (result.isConfirmed) {
            this.confirmPaymentSimulation(reservationId, response.transactionId);
          }
        });
        
        this.cd.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        console.error(err);
        Swal.fire('Error', 'No se pudo iniciar el pago.', 'error');
        this.cd.detectChanges();
      }
    });
  }

  // Método auxiliar para llamar al Webhook y confirmar la reserva real
  confirmPaymentSimulation(resId: string, transId: string) {
    this.isLoading = true;
    this.paymentService.simulateWebhook(resId, transId).subscribe({
      next: () => {
        Swal.fire('Pago Exitoso', 'Tu reserva ha sido confirmada.', 'success');
        this.loadReservations(); // Recargar la tabla para ver el estado "Confirmed"
      },
      error: () => {
        this.isLoading = false;
        Swal.fire('Error', 'El pago pasó pero el sistema no se actualizó.', 'warning');
        this.cd.detectChanges();
      }
    });
  }

  cancelReservation(id: string) {
    Swal.fire({
      title: '¿Cancelar Reserva?',
      text: "Esta acción no se puede deshacer.",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      confirmButtonText: 'Sí, cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.reservationService.cancelReservation(id).subscribe({
          next: () => {
            Swal.fire('Cancelada', 'La reserva ha sido cancelada.', 'success');
            this.loadReservations(); // Recargar lista
          },
          error: (err) => {
            Swal.fire('Error', 'No se pudo cancelar la reserva', 'error');
          }
        });
      }
    });
  }

  // Helper para asignar color según el estado
  getStatusClass(status: string): string {
    switch (status) {
      case 'Confirmed': return 'bg-success';
      case 'Pending': return 'bg-warning text-dark';
      case 'Canceled': return 'bg-danger';
      default: return 'bg-secondary';
    }
  }
}