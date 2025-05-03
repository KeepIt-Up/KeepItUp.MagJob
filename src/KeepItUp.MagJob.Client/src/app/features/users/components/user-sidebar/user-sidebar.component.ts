import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import {
  SidebarComponent,
  NavSection,
  NavItem,
} from '@shared/components/sidebar/sidebar.component';
import { AsyncPipe } from '@angular/common';
import { UserContextService } from '@users/services/user-context.service';

@Component({
  selector: 'app-user-sidebar',
  templateUrl: './user-sidebar.component.html',
  standalone: true,
  imports: [SidebarComponent, AsyncPipe],
})
export class UserSidebarComponent {
  @Input() sidebarExpanded = false;
  @Output() sidebarExpandedChange = new EventEmitter<boolean>();

  readonly userContextService = inject(UserContextService);

  mainSection: NavSection = {
    title: 'Main',
    items: [
      { path: 'organizations', icon: 'heroBuildingOffice', label: 'Organizations' },
      { path: 'invitations', icon: 'heroEnvelope', label: 'Invitations' },
    ],
  };

  settingsSection: NavSection = {
    title: 'Settings',
    items: [
      { path: 'settings', icon: 'heroUser', label: 'Profile' },
      { path: 'security/password', icon: 'heroShieldCheck', label: 'Security' },
    ],
  };

  footerItems: NavItem[] = [
    { path: 'help', icon: 'heroQuestionMarkCircle', label: 'Help' },
    { path: 'logout', icon: 'heroArrowRightOnRectangle', label: 'Sign Out' },
  ];
}
