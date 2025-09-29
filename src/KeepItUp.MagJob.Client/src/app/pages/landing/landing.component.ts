import { Component, inject } from '@angular/core';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { FooterComponent } from '../../shared/components/footer/footer.component';
import { NgIcon } from '@ng-icons/core';
import { AuthService } from '@core/services/auth.service';

@Component({
  selector: 'app-landing',
  imports: [NavbarComponent, ButtonComponent, NgIcon, FooterComponent],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss',
})
export class LandingComponent {
  readonly authService = inject(AuthService);
}
