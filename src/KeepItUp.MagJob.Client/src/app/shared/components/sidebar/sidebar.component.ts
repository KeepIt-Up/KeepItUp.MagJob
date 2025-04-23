import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { SafeUrl } from '@angular/platform-browser';
import { NgIcon } from '@ng-icons/core';
import { AuthService } from '@core/services/auth.service';

export interface NavItem {
  path: string;
  icon: string;
  label: string;
}

export interface NavSection {
  title: string;
  items: NavItem[];
}

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, NgIcon],
})
export class SidebarComponent {
  @Input() title = '';
  @Input() titlePath = '';
  @Input() logoUrl?: SafeUrl;
  @Input() sections: NavSection[] = [];
  @Input() footerItems: NavItem[] = [];
  @Input() sidebarExpanded = true;
  @Output() sidebarExpandedChange = new EventEmitter<boolean>();

  readonly authService = inject(AuthService);

  toggle() {
    this.sidebarExpanded = !this.sidebarExpanded;
    this.sidebarExpandedChange.emit(this.sidebarExpanded);
  }
}
