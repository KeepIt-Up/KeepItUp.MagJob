import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { Location } from '@angular/common';
import { FormsModule } from '@angular/forms';

// PrimeNG imports
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { InputTextModule } from 'primeng/inputtext';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
import { AuthService } from '@core/auth/services/auth.service';

@Component({
  selector: 'app-not-found',
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    ButtonModule,
    CardModule,
    DividerModule,
    InputTextModule,
    InputGroupModule,
    InputGroupAddonModule,
  ],
  templateUrl: './not-found.component.html',
})
export class NotFoundComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private location = inject(Location);

  searchTerm = '';

  // Suggested links for navigation
  suggestedLinks = [
    { label: 'Home', route: '/', icon: 'pi pi-home' },
    { label: 'Help Center', route: '/help', icon: 'pi pi-question-circle' },
    { label: 'Contact Support', route: '/contact', icon: 'pi pi-envelope' },
  ];

  goHome(): void {
    this.router.navigate(['/']);
  }

  goBack(): void {
    this.location.back();
  }

  signIn(): void {
    this.authService.initLoginFlow();
  }

  register(): void {
    this.authService.initRegistrationFlow();
  }

  contactSupport(): void {
    this.router.navigate(['/help']);
  }

  reportIssue(): void {
    this.router.navigate(['/help']);
  }
}
