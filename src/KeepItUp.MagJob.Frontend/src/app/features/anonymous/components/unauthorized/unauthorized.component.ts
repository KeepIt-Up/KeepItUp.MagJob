import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { Location } from '@angular/common';

import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { AuthService } from '@core/auth/services/auth.service';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [CommonModule, RouterLink, ButtonModule, CardModule, DividerModule],
  templateUrl: './unauthorized.component.html',
})
export class UnauthorizedComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private location = inject(Location);

  signIn(): void {
    this.authService.initLoginFlow();
  }

  register(): void {
    this.authService.initRegistrationFlow();
  }

  goHome(): void {
    this.router.navigate(['/']);
  }

  goBack(): void {
    this.location.back();
  }

  contactAdmin(): void {
    this.router.navigate(['/help']);
  }

  requestAccess(): void {
    this.router.navigate(['/help']);
  }
}
