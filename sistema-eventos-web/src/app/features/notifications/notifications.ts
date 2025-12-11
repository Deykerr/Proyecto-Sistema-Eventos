import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification.service';
import { AppNotification } from '../../core/models/notification.model';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notifications.html',
  styleUrl: './notifications.css'
})
export class NotificationsComponent implements OnInit {
  private notificationService = inject(NotificationService);
  private cd = inject(ChangeDetectorRef);

  notifications: AppNotification[] = [];
  isLoading = true;

  ngOnInit(): void {
    this.notificationService.getUserNotifications().subscribe({
      next: (data) => {
        this.notifications = data;
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
}