import { Component, inject, Input, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { OrganizationSidebarComponent } from '@organizations/components/organization-sidebar/organization-sidebar.component';
import { NavbarComponent } from '@shared/components/navbar/navbar.component';
import { ErrorAlertComponent } from '@shared/components/error-alert/error-alert.component';
import { SpinnerComponent } from '@shared/components/spinner/spinner.component';
import { OrganizationService } from '@organizations/services/organization.service';
import { ScrollControlService } from '@shared/services/scroll-control.service';

@Component({
  selector: 'app-organization',
  imports: [
    RouterOutlet,
    OrganizationSidebarComponent,
    NavbarComponent,
    ErrorAlertComponent,
    SpinnerComponent,
  ],
  templateUrl: './organization.component.html',
})
export class OrganizationComponent implements OnInit {
  @Input() organizationId!: string;
  private organizationService = inject(OrganizationService);
  private scrollControlService = inject(ScrollControlService);

  state$ = this.organizationService.state$;
  sidebarExpanded = false;
  scrollable = this.scrollControlService.scrollable$;

  ngOnInit(): void {
    this.organizationService.getOrganization(this.organizationId).subscribe();
  }

  sidebarExpandedChange(expanded: boolean) {
    this.sidebarExpanded = expanded;
  }
}
