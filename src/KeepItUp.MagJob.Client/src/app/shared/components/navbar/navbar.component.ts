import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DropdownComponent } from '../dropdown/dropdown.component';
import { RouterLink } from '@angular/router';
import { ClickOutsideDirective } from '../../directives/click-outside.directive';
import { UserBase } from '@core/models/user-base.model';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, DropdownComponent, RouterLink, ClickOutsideDirective],
  templateUrl: './navbar.component.html',
})
export class NavbarComponent implements OnInit {
  isDropdownOpen = false;
  isMobileMenuOpen = false;
  user: UserBase | null = null;
  isDarkMode = false;

  readonly authService = inject(AuthService);

  ngOnInit(): void {
    this.authService.getUserProfile().subscribe((user: UserBase | null) => {
      this.user = user;
    });

    // Check if dark mode is already enabled
    this.isDarkMode = document.documentElement.classList.contains('dark');

    // Check user preference
    if (
      localStorage.getItem('color-theme') === 'dark' ||
      (!localStorage.getItem('color-theme') &&
        window.matchMedia('(prefers-color-scheme: dark)').matches)
    ) {
      document.documentElement.classList.add('dark');
      this.isDarkMode = true;
    } else {
      document.documentElement.classList.remove('dark');
      this.isDarkMode = false;
    }
  }

  toggleDarkMode() {
    // Toggle dark mode
    this.isDarkMode = !this.isDarkMode;

    // Update DOM
    if (this.isDarkMode) {
      document.documentElement.classList.add('dark');
      localStorage.setItem('color-theme', 'dark');
    } else {
      document.documentElement.classList.remove('dark');
      localStorage.setItem('color-theme', 'light');
    }
  }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  toggleMobileMenu() {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  login() {
    this.authService.login();
  }

  logout() {
    this.authService.logout();
  }

  closeDropdown() {
    this.isDropdownOpen = false;
  }
}
