export interface DashboardStats {
  totalEvents: number;
  totalReservations: number;
  totalRevenue: number;
  activeEvents: number;
}

export interface EventStats {
  eventId: string;
  eventTitle: string;
  totalReservations: number;
  occupancyRate: number;
  revenue: number;
}