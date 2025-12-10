import { UserRole } from './enums.model';

export interface UserProfile {
  id: string; // GUID siempre es string en TS
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  createdAt: Date | string; // La API devuelve string ISO
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
}