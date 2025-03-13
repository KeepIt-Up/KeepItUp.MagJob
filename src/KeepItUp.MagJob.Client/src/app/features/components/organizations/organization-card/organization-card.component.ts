import { Component, inject, Input } from '@angular/core';
import { ImageService } from '@shared/services/image.service';
import { Organization } from '../../../models/organization/organization';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-organization-card',
  imports: [RouterLink],
  templateUrl: './organization-card.component.html'
})
export class OrganizationCardComponent {
  @Input() organization!: Organization;
  imageService = inject(ImageService);
}
