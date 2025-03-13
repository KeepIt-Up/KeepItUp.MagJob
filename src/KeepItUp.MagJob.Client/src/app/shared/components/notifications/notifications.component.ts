import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../../services/notification.service';
import { Notification } from '../../models/notification.model';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-notifications',
    imports: [CommonModule],
    templateUrl: './notifications.component.html',
    styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
    notifications: Notification[] = [];

    constructor(private notificationService: NotificationService) { }

    ngOnInit(): void {
        this.notificationService.getNotifications()
            .subscribe(notifications => {
                this.notifications = notifications;
            });
    }

    remove(id: string): void {
        this.notificationService.remove(id);
    }
} 