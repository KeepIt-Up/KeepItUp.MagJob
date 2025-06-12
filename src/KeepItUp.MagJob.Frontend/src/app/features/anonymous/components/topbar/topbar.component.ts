import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

// PrimeNG imports
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { AvatarModule } from 'primeng/avatar';
import { SidebarModule } from 'primeng/sidebar';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { TooltipModule } from 'primeng/tooltip';
import { DividerModule } from 'primeng/divider';

// PrimeNG icons
import { MenuItem } from 'primeng/api';

// Services
import { ThemeService } from '@shared/layout/services/theme.service';
import { AuthService } from '@core/auth/services/auth.service';

@Component({
  selector: 'app-topbar',
  imports: [
    CommonModule,
    RouterLink,
    ToolbarModule,
    ButtonModule,
    MenuModule,
    AvatarModule,
    SidebarModule,
    ToggleButtonModule,
    TooltipModule,
    DividerModule,
  ],
  templateUrl: './topbar.component.html',
})
export class TopbarComponent {
  private themeService = inject(ThemeService);
  private authService = inject(AuthService);

  // Signals for reactive state
  isUserMenuVisible = signal(false);
  isMobileMenuVisible = signal(false);
  isAuthenticated = signal(false);

  // User menu items
  userMenuItems: MenuItem[] = [
    {
      label: 'Profile',
      icon: 'pi pi-user',
      routerLink: '/user/profile',
    },
    {
      label: 'Settings',
      icon: 'pi pi-cog',
      routerLink: '/user/settings',
    },
    {
      separator: true,
    },
    {
      label: 'Sign out',
      icon: 'pi pi-sign-out',
      command: () => this.logout(),
    },
  ];

  // Mobile navigation items
  mobileMenuItems: MenuItem[] = [
    {
      label: 'My Organizations',
      icon: 'pi pi-building',
      routerLink: '/user/organizations',
    },
  ];

  isDarkMode = this.themeService.isDarkMode;

  toggleDarkMode(): void {
    this.themeService.toggleTheme();
  }

  toggleUserMenu(): void {
    this.isUserMenuVisible.update(value => !value);
  }

  toggleMobileMenu(): void {
    this.isMobileMenuVisible.update(value => !value);
  }

  login(): void {
    this.authService.initLoginFlow();
  }

  register(): void {
    this.authService.initRegistrationFlow();
  }

  logout(): void {
    this.authService.logOut();
    this.isUserMenuVisible.set(false);
  }

  // Mock user data - TODO: Replace with actual user service
  get currentUser(): {
    firstName?: string;
    lastName?: string;
    email?: string;
    avatarUrl?: string | null;
  } {
    return {
      firstName: 'John',
      lastName: 'Doe',
      email: 'john.doe@example.com',
      avatarUrl: null,
    };
  }
}
