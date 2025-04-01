import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ImageUploadModalComponent } from '@shared/components/image-upload-modal/image-upload-modal.component';
import { Subject, Subscription } from 'rxjs';
import { Organization } from '../../../features/organizations/models/organization.model';
import { OrganizationService } from '../../../features/organizations/services/organization.service';
import { ImageService } from '@shared/services/image.service';
import { NotificationService } from '@shared/services/notification.service';
import { UpdateOrganizationPayload } from '../../../features/organizations/services/organization.api.service';
import { SafeUrl } from '@angular/platform-browser';
import { useOrganization } from '@organizations/hooks/use-organization';
import { InputComponent } from '@shared/components/input/input.component';
import { ButtonComponent } from '@shared/components/button/button.component';
import { TextareaComponent } from '@shared/components/textarea/textarea.component';
import { CommonModule } from '@angular/common';
import { OrganizationContextService } from '@organizations/services/organization-context.service';

interface OrganizationForm {
  organizationName: FormControl<string | null>;
  organizationDescription: FormControl<string | null>;
}

@Component({
  selector: 'app-organization-profil',
  imports: [
    ReactiveFormsModule,
    ImageUploadModalComponent,
    InputComponent,
    ButtonComponent,
    TextareaComponent,
    CommonModule,
  ],
  templateUrl: './organization-profil.component.html',
})
export class OrganizationProfilComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private subscriptions = new Subscription();

  isProfileModalOpen = false;
  isBannerModalOpen = false;
  isResponseModalOpen = false;
  organization!: Organization;
  organizationId!: string;
  organizationForm!: FormGroup<OrganizationForm>;
  successMessage = '';
  errorMessage = '';
  loading = false;

  private organizationService = inject(OrganizationService);
  private organizationContextService = inject(OrganizationContextService);
  private notificationService = inject(NotificationService);
  private readonly organizationContext = useOrganization();

  ngOnInit(): void {
    this.initForm();
    this.loadOrganizationData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.subscriptions.unsubscribe();
  }

  private initForm(): void {
    // Use simplest form to avoid validator issues
    this.organizationForm = new FormGroup<OrganizationForm>({
      organizationName: new FormControl(''),
      organizationDescription: new FormControl(null),
    });

    // Add validators after creation
    const nameControl = this.organizationForm.get('organizationName');
    if (nameControl) {
      nameControl.setValidators([Validators.required, Validators.minLength(3)]);
    }
  }

  private loadOrganizationData(): void {
    this.loading = true;

    // Subscribe to organization context
    const orgSubscription = this.organizationContext.organization$.subscribe({
      next: org => {
        if (org) {
          this.organization = org as Organization;
          this.organizationId = org.id.toString();
          this.organizationForm.patchValue({
            organizationName: org.name,
            organizationDescription: org.description,
          });
        }
        this.loading = false;
      },
      error: (err: unknown) => {
        this.errorMessage = 'Failed to load organization data';
        console.error('Error loading organization data:', err);
        this.loading = false;
      },
    });

    this.subscriptions.add(orgSubscription);

    // Subscribe to loading state
    const loadingSubscription = this.organizationContext.loading$.subscribe(isLoading => {
      this.loading = isLoading;
    });

    this.subscriptions.add(loadingSubscription);

    // Subscribe to error state
    const errorSubscription = this.organizationContext.error$.subscribe(error => {
      if (error) {
        this.errorMessage = typeof error === 'string' ? error : 'Failed to load organization data';
      } else {
        this.errorMessage = '';
      }
    });

    this.subscriptions.add(errorSubscription);
  }

  onSubmit(): void {
    if (this.organizationForm.valid) {
      this.successMessage = '';
      this.errorMessage = '';
      this.loading = true;
      this.isResponseModalOpen = true;

      const payload: UpdateOrganizationPayload = {
        name: this.organizationForm.get('organizationName')?.value ?? '',
        description: this.organizationForm.get('organizationDescription')?.value ?? '',
      };

      const updateSubscription = this.organizationService
        .updateOrganization(this.organizationId, payload)
        .subscribe({
          next: (updatedOrg: Organization) => {
            this.successMessage = 'Organization updated successfully';

            // Update organization context with updated data
            this.organizationContextService.updateOrganizationContext(updatedOrg);

            // Update local organization data
            if (updatedOrg) {
              this.organization = updatedOrg;
            }

            this.isResponseModalOpen = false;
            this.loading = false;
            this.organizationForm.markAsPristine();
          },
          error: (err: unknown) => {
            this.errorMessage = 'Failed to update organization profile';
            console.error('Error updating organization:', err);
            this.isResponseModalOpen = false;
            this.loading = false;
          },
        });

      this.subscriptions.add(updateSubscription);
    }
  }

  openProfileDialog(): void {
    this.isProfileModalOpen = true;
  }

  openBannerDialog(): void {
    this.isBannerModalOpen = true;
  }

  handleProfileUpload(file: File): void {
    if (!this.validateFile(file)) {
      this.errorMessage = 'Invalid file type or size';
      this.notificationService.show('Invalid file type or size', 'error');
      return;
    }

    this.successMessage = '';
    this.errorMessage = '';
    this.loading = true;
    this.isResponseModalOpen = true;

    // Use the direct file upload method instead of base64 encoding
    const uploadSubscription = this.organizationService
      .updateLogo(this.organizationId, file)
      .subscribe({
        next: (response: { logoUrl: string }) => {
          this.successMessage = 'Organization logo updated successfully';

          // Update the organization with the new logo URL
          if (this.organization) {
            const updatedOrg = {
              ...this.organization,
              logoUrl: response.logoUrl,
            };

            // Update organization context
            this.organizationContextService.updateOrganizationContext(updatedOrg);

            // Update local data
            this.organization = updatedOrg;
          }

          this.isResponseModalOpen = false;
          this.loading = false;
        },
        error: (err: unknown) => {
          this.errorMessage = 'Failed to update profile image';
          console.error('Error updating logo:', err);
          this.isResponseModalOpen = false;
          this.loading = false;
        },
      });

    this.subscriptions.add(uploadSubscription);
  }

  handleBannerUpload(file: File): void {
    if (!this.validateFile(file)) {
      this.errorMessage = 'Invalid file type or size';
      this.notificationService.show('Invalid file type or size', 'error');
      return;
    }

    this.successMessage = '';
    this.errorMessage = '';
    this.loading = true;
    this.isResponseModalOpen = true;

    // Use the direct file upload method instead of base64 encoding
    const uploadSubscription = this.organizationService
      .updateBanner(this.organizationId, file)
      .subscribe({
        next: (response: { bannerUrl: string }) => {
          this.successMessage = 'Organization banner updated successfully';

          // Update the organization with the new banner URL
          if (this.organization) {
            const updatedOrg = {
              ...this.organization,
              bannerUrl: response.bannerUrl,
            };

            // Update organization context
            this.organizationContextService.updateOrganizationContext(updatedOrg);

            // Update local data
            this.organization = updatedOrg;
          }

          this.isResponseModalOpen = false;
          this.loading = false;
        },
        error: (err: unknown) => {
          this.errorMessage = 'Failed to update banner image';
          console.error('Error updating banner:', err);
          this.isResponseModalOpen = false;
          this.loading = false;
        },
      });

    this.subscriptions.add(uploadSubscription);
  }

  private validateFile(file: File): boolean {
    const validTypes = ['image/jpeg', 'image/png', 'image/gif'];
    const maxSize = 10 * 1024 * 1024; // 10MB

    if (!validTypes.includes(file.type)) {
      console.error('Invalid file type');
      return false;
    }

    if (file.size > maxSize) {
      console.error('File too large');
      return false;
    }

    return true;
  }

  getErrorMessage(controlName: string): string {
    const control = this.organizationForm.get(controlName);
    if (!control?.errors || !control.touched) {
      return '';
    }

    if (control.errors['required']) {
      return 'This field is required';
    }

    if (control.errors['minlength']) {
      return `Minimum length is ${control.errors['minlength'].requiredLength} characters`;
    }

    return 'Invalid value';
  }

  resetForm(): void {
    if (this.organization) {
      this.organizationForm.patchValue({
        organizationName: this.organization.name,
        organizationDescription: this.organization.description,
      });
      this.organizationForm.markAsPristine();
      this.successMessage = '';
      this.errorMessage = '';
    }
  }
}
