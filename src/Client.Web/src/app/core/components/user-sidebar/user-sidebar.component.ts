import { Component, computed, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from '../../../features/services/user.service';

@Component({
  selector: 'app-user-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './user-sidebar.component.html'
})
export class UserSidebarComponent {
  @Output() sidebarExpandedChange = new EventEmitter<boolean>();
  private authService = inject(OAuthService);
  private userService = inject(UserService);
  sidebarExpanded: boolean = true;

  toggle() {
    this.sidebarExpanded = !this.sidebarExpanded;
    this.sidebarExpandedChange.emit(this.sidebarExpanded);
  }

  state$ = this.userService.userState$;
  userName = computed(() => `${this.state$().data?.firstname} ${this.state$().data?.lastname}`);

  logOut() {
    this.authService.logOut();
  }
}
