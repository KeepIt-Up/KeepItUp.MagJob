import { Component, inject } from '@angular/core';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { RouterOutlet } from '@angular/router';
import { UserSidebarComponent } from '../../features/users/components/user-sidebar/user-sidebar.component';
import { SpinnerComponent } from '@shared/components/spinner/spinner.component';
import { ErrorAlertComponent } from '@shared/components/error-alert/error-alert.component';
import { UserService } from '../../features/users/services/user.service';

@Component({
  selector: 'app-user',
  imports: [
    NavbarComponent,
    RouterOutlet,
    UserSidebarComponent,
    SpinnerComponent,
    ErrorAlertComponent,
  ],
  templateUrl: './user.component.html',
})
export class UserComponent {
  private userService = inject(UserService);
  state$ = this.userService.userState$;
  sidebarExpanded = true;

  sidebarExpandedChange(expanded: boolean) {
    this.sidebarExpanded = expanded;
  }
}
