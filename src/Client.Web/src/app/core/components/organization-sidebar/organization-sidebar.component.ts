import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Organization } from '../../../features/models/organization/organization';
import { OAuthService } from 'angular-oauth2-oidc';
import { ImageService } from '@shared/services/image.service';
import { SafeUrl } from '@angular/platform-browser';

interface NavItem {
  path: string;
  icon: string;
  label: string;
}

@Component({
  selector: 'app-organization-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './organization-sidebar.component.html'
})
export class OrganizationSidebarComponent {
  @Input() organization!: Organization;
  sidebarExpanded!: boolean;
  @Output() sidebarExpandedChange = new EventEmitter<boolean>();

  private imageService = inject(ImageService);

  private authService = inject(OAuthService);

  mainNavItems: NavItem[] = [
    { path: 'dashboard', icon: 'bi-house', label: 'Home' },
    { path: 'members', icon: 'bi-people', label: 'Members' },
    //{ path: 'schedule', icon: 'bi-calendar', label: 'Schedule' },
  ];

  settingsNavItems: NavItem[] = [
    { path: 'settings', icon: 'bi-gear', label: 'Settings' },
  ];

  toggle() {
    this.sidebarExpanded = !this.sidebarExpanded;
    this.sidebarExpandedChange.emit(this.sidebarExpanded);
  }

  getSafeImageUrl(base64String: string | undefined): SafeUrl | undefined {
    return this.imageService.getSafeImageUrl(base64String);
  }

  logout() {
    this.authService.logOut();
  }

}
