import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { CurrentUser } from '@users/models/current-user.model';
import { UserContextService } from '@users/services/user-context.service';
import { InputComponent } from '@shared/components/input/input.component';
import { ButtonComponent } from '@shared/components/button/button.component';
import { UserService } from '@users/services/user.service';

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
  loading = false;
  successMessage = '';
  errorMessage = '';
  private subscriptions = new Subscription();

  private userContextService = inject(UserContextService);
  private userService = inject(UserService);

  $user = this.userContextService.$userContext;

  ngOnInit(): void {
    this.initForm();
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

  onSubmit(): void {
    if (this.userForm.valid && this.$user().data) {
      this.loading = true;
      this.successMessage = '';
      this.errorMessage = '';

      const formValues: UserFormValues = this.userForm.value;

      const updateRequest = {
        id: this.$user().data!.id,
        firstName: formValues.firstName,
        lastName: formValues.lastName,
        phoneNumber: formValues.phoneNumber,
        address: formValues.address,
      };

      const updateSubscription = this.userService.updateUser(updateRequest).subscribe({
        next: (updatedUser: CurrentUser) => {
          this.loading = false;
          this.successMessage = 'Profile updated successfully';

          // If API didn't return profileImageUrl (or it's null/undefined),
          // use the existing profile image URL
          if (!updatedUser.profileImageUrl && this.$user().data) {
            updatedUser = {
              ...updatedUser,
              profileImageUrl: this.$user().data!.profileImageUrl,
            };
          }

          this.userContextService.updateUserContext(updatedUser);
        },
        error: (err: Error) => {
          this.loading = false;
          this.errorMessage = err.message ?? 'Failed to update profile';
          console.error('Error updating profile:', err);
        },
      });

      this.subscriptions.add(updateSubscription);
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length || !this.$user().data) {
      return;
    }

    const file = input.files[0];
    if (!file.type.includes('image/')) {
      this.errorMessage = 'Please select an image file';
      return;
    }

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    const formData = new FormData();
    formData.append('profilePictureFile', file);

    // Store current user ID to ensure it's available
    const userId = this.$user().data!.id;

    const uploadSubscription = this.userService.updateProfilePicture(userId, formData).subscribe({
      next: (response: { profileImageUrl?: string }) => {
        this.loading = false;
        this.successMessage = 'Profile picture updated successfully';

        // Since the API only returns profileImageUrl, we need to preserve the rest of the user data
        if (this.$user().data) {
          // Create updated user object by merging current user data with new profile image URL
          const updatedUser: CurrentUser = {
            ...this.$user().data!,
            profileImageUrl: response.profileImageUrl ?? this.$user().data!.profileImageUrl,
          };

          // Update user context with merged data
          this.userContextService.updateUserContext(updatedUser);
        }
      },
      error: (err: Error) => {
        this.loading = false;
        this.errorMessage = err.message ?? 'Failed to update profile picture';
        console.error('Error updating profile picture:', err);
      },
    });

    this.subscriptions.add(uploadSubscription);
  }

  resetForm(): void {
    if (this.$user().data) {
      this.userForm.patchValue({
        email: this.$user().data!.email,
        firstName: this.$user().data!.firstName,
        lastName: this.$user().data!.lastName,
        phoneNumber: this.$user().data!.phoneNumber ?? '',
        address: this.$user().data!.address ?? '',
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
