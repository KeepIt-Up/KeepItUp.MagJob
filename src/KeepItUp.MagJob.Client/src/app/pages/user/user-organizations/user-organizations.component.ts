import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HeaderComponent } from '@shared/components/header/header.component';
import { InfiniteListComponent } from '@shared/components/infinite-list/infinite-list.component';
import { UserService } from '../../../features/users/services/user.service';
import { OrganizationCardComponent } from '../../../features/organizations/components/organization-card/organization-card.component';
import { ErrorAlertComponent } from '../../../shared/components/error-alert/error-alert.component';
import { ButtonComponent } from '@shared/components/button/button.component';

@Component({
  selector: 'app-user-organizations',
  standalone: true,
  imports: [
    HeaderComponent,
    CommonModule,
    RouterLink,
    InfiniteListComponent,
    OrganizationCardComponent,
    ErrorAlertComponent,
    ButtonComponent,
  ],
  templateUrl: './user-organizations.component.html',
})
export class UserOrganizationsComponent {
  private userService = inject(UserService);

  organizationState$ = this.userService.organizationState$;
  paginationOptions$ = this.userService.organizationsPaginationOptions$;

  loadMore(): void {
    this.userService.getUserOrganizations().subscribe();
  }
}
