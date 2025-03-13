import { Component, OnInit, inject } from '@angular/core';
import { SearchInputComponent } from '../../../shared/components/search-input/search-input.component';
import { CommonModule } from '@angular/common';
import { DropdownComponent } from '../../../shared/components/dropdown/dropdown.component';
import { RouterLink } from '@angular/router';
import { ClickOutsideDirective } from '../../../shared/directives/click-outside.directive';
import { User } from '@features/models/user/user';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-navbar',
  imports: [
    SearchInputComponent,
    CommonModule,
    DropdownComponent,
    RouterLink,
    ClickOutsideDirective,
  ],
  templateUrl: './navbar.component.html',
})
export class NavbarComponent implements OnInit {
  isDropdownOpen = false;
  userInfo: User | null = null;

  readonly authService = inject(AuthService);

  ngOnInit(): void {
    this.authService.getUserProfile().subscribe((user: User) => {
      this.userInfo = user;
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
