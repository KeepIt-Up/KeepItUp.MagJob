import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DropdownComponent } from '../../../shared/components/dropdown/dropdown.component';
import { RouterLink } from '@angular/router';
import { ClickOutsideDirective } from '../../../shared/directives/click-outside.directive';
import { UserBase } from '@core/models/user-base.model';
import { AuthService } from '../../../core/services/auth.service';
@Component({
  selector: 'app-navbar',
  imports: [CommonModule, DropdownComponent, RouterLink, ClickOutsideDirective],
  templateUrl: './navbar.component.html',
})
export class NavbarComponent implements OnInit {
  isDropdownOpen = false;
  user: UserBase | null = null;

  readonly authService = inject(AuthService);

  ngOnInit(): void {
    this.authService.getUserProfile().subscribe((user: UserBase | null) => {
      this.user = user;
    });
  }

  toggleDarkMode() {
    const html = document.documentElement;
    html.classList.toggle('dark');
  }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  logout() {
    this.authService.logout();
  }

  closeDropdown() {
    this.isDropdownOpen = false;
  }
}
