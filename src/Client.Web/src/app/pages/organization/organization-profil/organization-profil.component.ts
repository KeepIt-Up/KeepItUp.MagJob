import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ImageUploadModalComponent } from '@shared/components/image-upload-modal/image-upload-modal.component';
import { Subject } from 'rxjs';
import { UpdateOrganizationPayload } from '@features/apis/organization.api.service';
import { ImageService } from '@shared/services/image.service';
import { Organization } from '@features/models/organization/organization';
import { effect } from '@angular/core';
import { NotificationService } from '@shared/services/notification.service';
import { SafeUrl } from '@angular/platform-browser';
import { OrganizationService } from '@features/services/organization.service';

@Component({
  selector: 'app-organization-profil',
  imports: [
    ReactiveFormsModule,
    ImageUploadModalComponent
  ],
  templateUrl: './organization-profil.component.html'
})
export class OrganizationProfilComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  isProfileModalOpen = false;
  isBannerModalOpen = false;
  isResponseModalOpen = false;
  organization!: Organization;

  organizationForm = new FormGroup({
    organizationName: new FormControl('', [Validators.required, Validators.minLength(3)]),
    organizationDescription: new FormControl<string | null>(null),
  });

  private organizationService = inject(OrganizationService);
  private notificationService = inject(NotificationService);
  private imageService = inject(ImageService);

  constructor() {
    effect(() => {
      const org = this.organizationService.state$().data;
      if (org) {
        this.organization = org;
        this.organizationId = org.id.toString();
        this.organizationForm.patchValue({
          organizationName: org.name,
          organizationDescription: org.description
        });
      }
    });
  }

  organizationId!: string;

  ngOnInit() {
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSubmit() {
    if (this.organizationForm.valid) {
      const payload: UpdateOrganizationPayload = {
        name: this.organizationForm.get('organizationName')?.value || '',
        description: this.organizationForm.get('organizationDescription')?.value || ''
      };

      this.isResponseModalOpen = true;
      this.organizationService.updateOrganization(
        this.organizationId,
        payload
      ).subscribe();
    }
  }

  openProfileDialog(): void {
    this.isProfileModalOpen = true;
  }

  openBannerDialog(): void {
    this.isBannerModalOpen = true;
  }

  handleProfileUpload(file: File): void {
    this.handleImageUpload(file, 'profileImage');
  }

  handleBannerUpload(file: File): void {
    this.handleImageUpload(file, 'bannerImage');
  }

  private handleImageUpload(file: File, imageType: 'profileImage' | 'bannerImage'): void {
    if (!this.validateFile(file)) {
      return;
    }

    this.isResponseModalOpen = true;
    const reader = new FileReader();

    reader.onload = () => {
      const base64String = (reader.result as string).split(',')[1];
      const payload: UpdateOrganizationPayload = {
        [imageType]: base64String
      };

      this.organizationService.updateOrganization(
        this.organizationId,
        payload
      ).subscribe();
    };

    reader.readAsDataURL(file);
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

  getSafeImageUrl(base64String: string | undefined): SafeUrl | undefined {
    return this.imageService.getSafeImageUrl(base64String);
  }
}
