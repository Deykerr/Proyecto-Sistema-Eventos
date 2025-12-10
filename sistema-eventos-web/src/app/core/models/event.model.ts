import { EventStatus } from './enums.model';

// Para listar y ver detalles (GET)
export interface Event {
  id: string;
  title: string;
  description: string;
  organizerName: string;
  categoryName: string;
  startDate: string; // ISO String
  endDate: string;   // ISO String
  location: string;
  latitude: number;  // decimal en C# es number en TS
  longitude: number;
  capacity: number;
  availableSlots: number;
  price: number;
  status: EventStatus;
  isPublic: boolean;
}

// Para crear (POST)
export interface CreateEventRequest {
  title: string;
  description: string;
  categoryId: string;
  startDate: string | Date;
  endDate: string | Date;
  location: string;
  latitude: number;
  longitude: number;
  capacity: number;
  price: number;
  isPublic: boolean;
}

// Para actualizar (PUT) - Hereda de Create pero añade status
export interface UpdateEventRequest extends CreateEventRequest {
  status: EventStatus;
}