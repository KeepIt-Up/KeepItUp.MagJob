import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AlertContainerComponent } from '@shared/components/alert-container/alert-container.component';
import { NotificationsComponent } from '@shared/components/notifications/notifications.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, AlertContainerComponent, NotificationsComponent],
  template: `
    <router-outlet></router-outlet>
    <app-alert-container></app-alert-container>
    <app-notifications></app-notifications>
  `,
  styles: [],
})
export class AppComponent {}
