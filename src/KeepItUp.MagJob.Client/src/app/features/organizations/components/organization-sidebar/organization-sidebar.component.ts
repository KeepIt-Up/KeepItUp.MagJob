import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { ImageService } from '@shared/services/image.service';
import { SafeUrl } from '@angular/platform-browser';
import { Organization } from '@organizations/models/organization.model';
import {
  SidebarComponent,
  NavSection,
  NavItem,
} from '@shared/components/sidebar/sidebar.component';

@Component({
  selector: 'app-organization-sidebar',
  imports: [SidebarComponent],
  templateUrl: './organization-sidebar.component.html',
  standalone: true,
})
export class OrganizationSidebarComponent {
  @Input() organization!: Organization;
  @Input() sidebarExpanded = true;
  @Output() sidebarExpandedChange = new EventEmitter<boolean>();

  private imageService = inject(ImageService);

  mainSection: NavSection = {
    title: 'Main',
    items: [
      { path: 'dashboard', icon: 'heroHome', label: 'Home' },
      { path: 'members', icon: 'heroUserGroup', label: 'Members' },
    ],
  };

  settingsSection: NavSection = {
    title: 'Settings',
    items: [
      { path: 'settings', icon: 'heroCog', label: 'Settings' },
      { path: 'roles', icon: 'heroUserGroup', label: 'Roles' },
    ],
  };

  footerItems: NavItem[] = [
    { path: 'help', icon: 'heroQuestionMarkCircle', label: 'Help' },
    { path: 'logout', icon: 'heroArrowRightOnRectangle', label: 'Sign Out' },
  ];

  getSafeImageUrl(base64String: string | undefined): SafeUrl | undefined {
    return this.imageService.getSafeImageUrl(base64String);
  }

  getLogoUrl(): SafeUrl | undefined {
    return this.getSafeImageUrl(this.organization?.logoUrl ?? this.organization?.profileImage);
  }
}
