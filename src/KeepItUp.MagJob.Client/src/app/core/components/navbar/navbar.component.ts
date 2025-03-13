import { Component, OnInit, inject } from '@angular/core';
import { SearchInputComponent } from '../../../shared/components/search-input/search-input.component';
import { CommonModule } from '@angular/common';
import { DropdownComponent } from '../../../shared/components/dropdown/dropdown.component';
import { RouterLink } from '@angular/router';
import { ClickOutsideDirective } from '../../../shared/directives/click-outside.directive';
import { OAuthService } from 'angular-oauth2-oidc';
@Component({
  selector: 'app-navbar',
  imports: [SearchInputComponent, CommonModule, DropdownComponent, RouterLink, ClickOutsideDirective],
  templateUrl: './navbar.component.html'
})
export class NavbarComponent implements OnInit {
  isDropdownOpen = false;
  userInfo: any;

  readonly authService = inject(OAuthService);

  ngOnInit(): void {
    this.userInfo = this.authService.getIdentityClaims();
  }

  toggleDarkMode() {
    const html = document.documentElement;
    html.classList.toggle('dark');
  }

  search(text: string) {
    console.log("Search for:", text);
  }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  logout() {
    this.authService.logOut();
  }

  closeDropdown() {
    this.isDropdownOpen = false;
  }

  getUserName(): string {
    return this.userInfo?.name || this.userInfo?.preferred_username || 'User';
  }

  getUserEmail(): string {
    return this.userInfo?.email || '';
  }
}
