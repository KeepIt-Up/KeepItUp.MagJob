import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HeaderComponent } from '@shared/components/header/header.component';
import { InfiniteListComponent } from '@shared/components/infinite-list/infinite-list.component';
import { UserService } from '@features/services/user.service';
import { OrganizationCardComponent } from '@features/components/organizations/organization-card/organization-card.component';
import { ErrorAlertComponent } from "../../../shared/components/error-alert/error-alert.component";

@Component({
  selector: 'app-user-organizations',
  imports: [HeaderComponent, CommonModule, RouterLink, InfiniteListComponent, OrganizationCardComponent, ErrorAlertComponent],
  templateUrl: './user-organizations.component.html'
})
export class UserOrganizationsComponent {

  private userService = inject(UserService);

  private userState$ = this.userService.userState$;
  organizationState$ = this.userService.organizationState$;
  paginationOptions$ = this.userService.organizationsPaginationOptions$;

  queryParams = computed(() => ({ id: this.userState$().data?.id }));

  loadMore(): void {
    this.userService.getUserOrganizations(this.queryParams()).subscribe();
  }
}
