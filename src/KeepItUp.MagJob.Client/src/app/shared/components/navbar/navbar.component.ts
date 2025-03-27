import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DropdownComponent } from '../dropdown/dropdown.component';
import { RouterLink } from '@angular/router';
import { ClickOutsideDirective } from '../../directives/click-outside.directive';
import { NgIcon } from '@ng-icons/core';
import { UserContextService } from '@users/services/user-context.service';
import { ThemeService } from '../../theme/theme.service';
import { AuthService } from '@core/services/auth.service';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, DropdownComponent, RouterLink, ClickOutsideDirective, NgIcon],
  templateUrl: './navbar.component.html',
})
export class NavbarComponent {
  readonly themeService = inject(ThemeService);
  readonly userContextService = inject(UserContextService);
  readonly authService = inject(AuthService);

  get userContext() {
    return {
      data: this.userContextService.getCurrentUser(),
      loading: false,
    };
  }

  isDropdownOpen = false;
  isMobileMenuOpen = false;

  toggleUserDropdown(): void {
    this.isDropdownOpen = !this.isDropdownOpen;
  }
}
