import { Component, inject } from '@angular/core';
import { NavbarComponent } from '@shared/components/navbar/navbar.component';
import { ButtonComponent } from '@shared/components/button/button.component';
import { NgIcon } from '@ng-icons/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '@core/services/auth.service';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [NavbarComponent, ButtonComponent, NgIcon, RouterLink],
  templateUrl: './unauthorized.component.html',
})
export class UnauthorizedComponent {
  readonly authService = inject(AuthService);
}
