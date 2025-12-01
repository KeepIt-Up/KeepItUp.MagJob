import { Injectable } from '@angular/core';
import { Notification, NotificationType } from '@shared/models/notification.model';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private notifications = new BehaviorSubject<Notification[]>([]);

  getNotifications(): Observable<Notification[]> {
    return this.notifications.asObservable();
  }

  show(message: string, type: NotificationType = 'info', duration = 3000, chatId?: string, organizationId?: string): void {
    const notification: Notification = {
      id: this.generateId(),
      message,
      type,
      duration,
      chatId,
      organizationId,
    };

    const currentNotifications = this.notifications.getValue();
    this.notifications.next([...currentNotifications, notification]);

    if (duration > 0) {
      setTimeout(() => {
        this.remove(notification.id);
      }, duration);
    }
  }

  remove(id: string): void {
    const currentNotifications = this.notifications.getValue();
    this.notifications.next(currentNotifications.filter(notification => notification.id !== id));
  }

  private generateId(): string {
    return Math.random().toString(36).substring(2, 9);
  }
}
