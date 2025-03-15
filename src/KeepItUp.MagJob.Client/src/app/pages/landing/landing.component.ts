import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { NgIcon } from '@ng-icons/core';
import { AuthService } from '@core/services/auth.service';

@Component({
  selector: 'app-landing',
  imports: [RouterLink, NavbarComponent, ButtonComponent, NgIcon],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss',
})
export class LandingComponent {
  constructor(private authService: AuthService) {}

  onLogin(): void {
    this.authService.login();
  }
}
