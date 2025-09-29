import { Component, inject } from '@angular/core';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { RouterOutlet } from '@angular/router';
import { UserSidebarComponent } from '../../features/users/components/user-sidebar/user-sidebar.component';
import { SpinnerComponent } from '@shared/components/spinner/spinner.component';
import { UserContextService } from '@users/services/user-context.service';
import { ErrorAlertComponent } from '@shared/components/error-alert/error-alert.component';

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
  private userContextService = inject(UserContextService);
  $userContext = this.userContextService.$userContext;
  sidebarExpanded = false;

  sidebarExpandedChange(expanded: boolean) {
    this.sidebarExpanded = expanded;
  }

  getError(error: string | undefined) {
    return new Error(error);
  }
}
