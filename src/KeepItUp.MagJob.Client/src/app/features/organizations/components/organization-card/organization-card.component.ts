import { Component, inject, Input } from '@angular/core';
import { ImageService } from '@shared/services/image.service';
import { Organization } from '../../models/organization.model';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from '@shared/components/button/button.component';

@Component({
  selector: 'app-organization-card',
  standalone: true,
  imports: [RouterLink, CommonModule, ButtonComponent],
  templateUrl: './organization-card.component.html',
})
export class OrganizationCardComponent {
  @Input() organization!: Organization;
  imageService = inject(ImageService);
}
