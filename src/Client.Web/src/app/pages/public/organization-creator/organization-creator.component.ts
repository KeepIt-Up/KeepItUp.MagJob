import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OrganizationService } from '@features/services/organization.service';
import { NotificationService } from '@shared/services/notification.service';

@Component({
  selector: 'app-organization-creator',
  imports: [FormsModule],
  templateUrl: './organization-creator.component.html'
})
export class OrganizationCreatorComponent {
  readonly organizationService = inject(OrganizationService);
  readonly notificationService = inject(NotificationService);
  organizationCreatePayload = {
    name: '',
    description: '',
  };

  onSubmit() {
    this.organizationService.createOrganization(this.organizationCreatePayload).subscribe();
  }
}
