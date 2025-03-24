import { Component, computed, EventEmitter, inject, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './user-sidebar.component.html',
})
export class UserSidebarComponent {
  @Output() sidebarExpandedChange = new EventEmitter<boolean>();
  private authService = inject(OAuthService);
  private userService = inject(UserService);
  sidebarExpanded = true;

  toggle() {
    this.sidebarExpanded = !this.sidebarExpanded;
    this.sidebarExpandedChange.emit(this.sidebarExpanded);
  }

  state$ = this.userService.userState$;
  userName = computed(() => `${this.state$().data?.given_name} ${this.state$().data?.family_name}`);

  logOut() {
    this.authService.logOut();
  }
}
