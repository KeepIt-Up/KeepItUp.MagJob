import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationService } from '../../services/notification.service';
import { Notification } from '../../models/notification.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notifications',
  imports: [CommonModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss'],
})
export class NotificationsComponent implements OnInit {
  notifications: Notification[] = [];
  private router = inject(Router);
  private notificationService = inject(NotificationService);

  ngOnInit(): void {
    this.notificationService.getNotifications().subscribe(notifications => {
      this.notifications = notifications;
    });
  }

  handleNotificationClick(notification: Notification): void {
    if (notification.chatId && notification.organizationId) {
      // Navigate to chat with query parameter
      this.router.navigate(['/organization', notification.organizationId, 'chats'], {
        queryParams: { chatId: notification.chatId },
      });
    }
    this.remove(notification.id);
  }

  remove(id: string): void {
    this.notificationService.remove(id);
  }
}
