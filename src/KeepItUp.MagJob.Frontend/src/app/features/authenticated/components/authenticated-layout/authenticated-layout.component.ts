import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthenticatedSidebarComponent } from '../authenticated-sidebar/authenticated-sidebar.component';
import { AuthenticatedTopbarComponent } from '../authenticated-topbar/authenticated-topbar.component';

@Component({
  selector: 'app-authenticated-layout',
  imports: [AuthenticatedTopbarComponent, RouterOutlet, AuthenticatedSidebarComponent],
  templateUrl: './authenticated-layout.component.html',
})
export class AuthenticatedLayoutComponent {}
