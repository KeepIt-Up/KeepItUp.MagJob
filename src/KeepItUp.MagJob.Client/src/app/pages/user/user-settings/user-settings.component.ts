import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { CurrentUser } from '@users/models/current-user.model';
import { UserContextService } from '@users/services/user-context.service';
import { InputComponent } from '@shared/components/input/input.component';
import { ButtonComponent } from '@shared/components/button/button.component';
import { UserService } from '@users/services/user.service';
import { AlertComponent } from '@shared/components/alert/alert.component';
import { AlertService } from '@shared/services/alert.service';

interface UserForm {
  email: FormControl<string | null>;
  firstName: FormControl<string | null>;
  lastName: FormControl<string | null>;
  phoneNumber: FormControl<string | null>;
  address: FormControl<string | null>;
}

interface UserFormValues {
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  address: string;
}

@Component({
  selector: 'app-user-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InputComponent, ButtonComponent],
  templateUrl: './user-settings.component.html',
})
export class UserSettingsComponent implements OnInit, OnDestroy {
  userForm!: FormGroup;
  user: CurrentUser | null = null;
  loading = false;
  successMessage = '';
  errorMessage = '';
  private subscriptions = new Subscription();

  private userContextService = inject(UserContextService);
  private userService = inject(UserService);
  private alertService = inject(AlertService);

  ngOnInit(): void {
    this.initForm();
    this.loadUserData();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private initForm(): void {
    this.userForm = new FormGroup<UserForm>({
      email: new FormControl({ value: '', disabled: true }),
      firstName: new FormControl('', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(50),
      ]),
      lastName: new FormControl('', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(50),
      ]),
      phoneNumber: new FormControl('', [
        Validators.maxLength(20),
        Validators.pattern(/^[+]?[(]?[0-9]{1,4}[)]?[-\s.]?[0-9]{1,4}[-\s.]?[0-9]{1,9}$/),
      ]),
      address: new FormControl('', [Validators.maxLength(200)]),
    });
  }

  private loadUserData(): void {
    this.loading = true;

    const userSubscription = this.userContextService.user$.subscribe({
      next: user => {
        if (user) {
          this.user = user;
          this.userForm.patchValue({
            email: user.email,
            firstName: user.firstName,
            lastName: user.lastName,
            phoneNumber: user.phoneNumber ?? '',
            address: user.address ?? '',
          });
        }
        this.loading = false;
      },
      error: (err: unknown) => {
        this.errorMessage = 'Failed to load user data';
        this.alertService.error('Error', 'Failed to load user data', {
          showActionButtons: true,
        });
        this.loading = false;
        console.error('Error loading user data:', err);
      },
    });

    this.subscriptions.add(userSubscription);

    // Reload user data if not already loaded
    if (!this.userContextService.getCurrentUser()) {
      this.userContextService.loadCurrentUser().subscribe();
    }
  }

  onSubmit(): void {
    if (this.userForm.valid && this.user) {
      this.loading = true;
      this.successMessage = '';
      this.errorMessage = '';

      const formValues: UserFormValues = this.userForm.value;

      const updateRequest = {
        id: this.user.id,
        firstName: formValues.firstName,
        lastName: formValues.lastName,
        phoneNumber: formValues.phoneNumber,
        address: formValues.address,
      };

      const updateSubscription = this.userService.updateUser(updateRequest).subscribe({
        next: (updatedUser: CurrentUser) => {
          this.loading = false;
          this.successMessage = 'Profile updated successfully';
          this.alertService.success('Success', 'Profile updated successfully', {
            autoHideTimeout: 5000,
            showActionButtons: true,
          });

          // If API didn't return profileImageUrl (or it's null/undefined),
          // use the existing profile image URL
          if (!updatedUser.profileImageUrl && this.user) {
            updatedUser = {
              ...updatedUser,
              profileImageUrl: this.user.profileImageUrl,
            };
          }

          this.userContextService.updateUserContext(updatedUser);
          this.user = updatedUser;
        },
        error: (err: Error) => {
          this.loading = false;
          this.errorMessage = err.message ?? 'Failed to update profile';
          this.alertService.error('Error', err.message ?? 'Failed to update profile', {
            showActionButtons: true,
          });
          console.error('Error updating profile:', err);
        },
      });

      this.subscriptions.add(updateSubscription);
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length || !this.user) {
      return;
    }

    const file = input.files[0];
    if (!file.type.includes('image/')) {
      this.errorMessage = 'Please select an image file';
      this.alertService.warning('Invalid File', 'Please select an image file', {
        showActionButtons: true,
      });
      return;
    }

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    const formData = new FormData();
    formData.append('profilePictureFile', file);

    // Store current user ID to ensure it's available
    const userId = this.user.id;

    const uploadSubscription = this.userService.updateProfilePicture(userId, formData).subscribe({
      next: (response: { profileImageUrl?: string }) => {
        this.loading = false;
        this.successMessage = 'Profile picture updated successfully';
        this.alertService.success('Success', 'Profile picture updated successfully', {
          autoHideTimeout: 5000,
          showActionButtons: true,
        });

        // Since the API only returns profileImageUrl, we need to preserve the rest of the user data
        if (this.user) {
          // Create updated user object by merging current user data with new profile image URL
          const updatedUser: CurrentUser = {
            ...this.user,
            profileImageUrl: response.profileImageUrl ?? this.user.profileImageUrl,
          };

          // Update user context with merged data
          this.userContextService.updateUserContext(updatedUser);
          this.user = updatedUser;
        }
      },
      error: (err: Error) => {
        this.loading = false;
        this.errorMessage = err.message ?? 'Failed to update profile picture';
        this.alertService.error('Error', err.message ?? 'Failed to update profile picture', {
          showActionButtons: true,
        });
        console.error('Error updating profile picture:', err);
      },
    });

    this.subscriptions.add(uploadSubscription);
  }

  resetForm(): void {
    if (this.user) {
      this.userForm.patchValue({
        email: this.user.email,
        firstName: this.user.firstName,
        lastName: this.user.lastName,
        phoneNumber: this.user.phoneNumber ?? '',
        address: this.user.address ?? '',
      });
      this.userForm.markAsPristine();
    }
  }

  getErrorMessage(controlName: string): string {
    const control = this.userForm.get(controlName);
    if (!control?.errors || !control.touched) {
      return '';
    }

    if (control.errors['required']) {
      return 'This field is required';
    }

    if (control.errors['minlength']) {
      return `Minimum length is ${control.errors['minlength'].requiredLength} characters`;
    }

    if (control.errors['maxlength']) {
      return `Maximum length is ${control.errors['maxlength'].requiredLength} characters`;
    }

    return 'Invalid value';
  }
}
