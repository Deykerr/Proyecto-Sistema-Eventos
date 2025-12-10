import { NotificationStatus, NotificationType } from './enums.model';

export interface AppNotification {
  id: string;
  subject: string;
  content: string;
  type: NotificationType | string;
  status: NotificationStatus | string;
  createdAt: string;
  sentAt?: string;
}