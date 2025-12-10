import { ReservationStatus } from './enums.model';

export interface CreateReservationRequest {
  eventId: string;
}

export interface Reservation {
  id: string;
  eventId: string;
  eventTitle: string;
  userName: string;
  reservationDate: string; // ISO String
  status: ReservationStatus;
  totalAmount: number;
}