import { Component, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { InputComponent } from '../../../shared/components/input/input.component';
import { TextareaComponent } from '../../../shared/components/textarea/textarea.component';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../shared/components/footer/footer.component';
import { NgIcon } from '@ng-icons/core';
import { OrganizationService } from '../../../features/organizations/services/organization.service';
import { CreateOrganizationPayload } from '../../../features/organizations/services/organization.api.service';
import { AuthService } from '@core/services/auth.service';

@Component({
  selector: 'app-create-organization',
  templateUrl: './create-organization.component.html',
  styleUrl: './create-organization.component.scss',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonComponent,
    InputComponent,
    TextareaComponent,
    RouterModule,
    NavbarComponent,
    FooterComponent,
    NgIcon,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class CreateOrganizationComponent {
  organizationForm: FormGroup;
  isSubmitting = false;

  private fb = inject(FormBuilder);
  private organizationService = inject(OrganizationService);
  private router = inject(Router);
  readonly authService = inject(AuthService);

  constructor() {
    this.organizationForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
    });
  }

  onSubmit = (): void => {
    if (this.organizationForm.invalid || this.isSubmitting) {
      return;
    }

    this.isSubmitting = true;

    const payload: CreateOrganizationPayload = {
      name: this.organizationForm.get('name')?.value,
      description: this.organizationForm.get('description')?.value,
    };

    this.organizationService.createOrganization(payload).subscribe({
      next: () => {
        // Navigate to the organization page
        this.router.navigate(['/user/organizations']);
      },
      error: (error: any) => {
        console.error('Error creating organization', error);
        this.isSubmitting = false;
      },
    });
  };
}
