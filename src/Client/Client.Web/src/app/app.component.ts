import { Component, computed, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { UserService } from '@features/services/user.service';
import { NotificationsComponent } from '@shared/components/notifications/notifications.component';
import { SpinnerComponent } from '@shared/components/spinner/spinner.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  imports: [RouterOutlet, NotificationsComponent, SpinnerComponent]
})
export class AppComponent {
  private userContext = inject(UserService);

  $user = computed(() => { return this.userContext.userState$().data; });
}
