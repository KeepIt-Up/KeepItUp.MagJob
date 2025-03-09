import { Component, inject } from '@angular/core';
import { NavbarComponent } from "@core/components/navbar/navbar.component";
import { RouterOutlet } from '@angular/router';
import { UserSidebarComponent } from '@core/components/user-sidebar/user-sidebar.component';
import { UserService } from '@features/services/user.service';
import { ErrorAlertComponent } from '@shared/components/error-alert/error-alert.component';
import { SpinnerComponent } from '@shared/components/spinner/spinner.component';

@Component({
    selector: 'app-user',
    imports: [
        NavbarComponent,
        RouterOutlet,
        UserSidebarComponent,
        SpinnerComponent,
        ErrorAlertComponent
    ],
    templateUrl: './user.component.html'
})
export class UserComponent {

  private userService = inject(UserService);
  state$ = this.userService.userState$;
  sidebarExpanded = true;

  sidebarExpandedChange(expanded: boolean) {
    this.sidebarExpanded = expanded;
  }
}
