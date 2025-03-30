import { Component, inject, Input, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { OrganizationSidebarComponent } from '@organizations/components/organization-sidebar/organization-sidebar.component';
import { NavbarComponent } from '@shared/components/navbar/navbar.component';
import { ErrorAlertComponent } from '@shared/components/error-alert/error-alert.component';
import { SpinnerComponent } from '@shared/components/spinner/spinner.component';
import { ScrollControlService } from '@shared/services/scroll-control.service';
import { OrganizationContextService } from '@organizations/services/organization-context.service';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-organization',
  imports: [
    RouterOutlet,
    OrganizationSidebarComponent,
    NavbarComponent,
    ErrorAlertComponent,
    SpinnerComponent,
    AsyncPipe,
  ],
  templateUrl: './organization.component.html',
})
export class OrganizationComponent implements OnInit {
  @Input() organizationId!: string;
  private organizationContextService = inject(OrganizationContextService);
  private scrollControlService = inject(ScrollControlService);

  organizationContext$ = this.organizationContextService.organizationContext$;
  sidebarExpanded = false;
  scrollable = this.scrollControlService.scrollable$;

  ngOnInit(): void {
    this.organizationContextService.loadOrganization(this.organizationId).subscribe();
  }

  sidebarExpandedChange(expanded: boolean) {
    this.sidebarExpanded = expanded;
  }
}
