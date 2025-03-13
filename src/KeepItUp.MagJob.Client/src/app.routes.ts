import { Routes } from '@angular/router';
import { InvitationTableComponent } from './app/pages/organization/invitation-table/invitation-table.component';
import { MembersTableComponent } from './app/pages/organization/members-table/member-table.component';
import { OrganizationProfilComponent } from './app/pages/organization/organization-profil/organization-profil.component';
import { OrganizationComponent } from './app/pages/organization/organization.component';
import { RolesManagementComponent } from './app/pages/organization/roles-management/roles-management.component';
import { UserInvitationsComponent } from './app/pages/user/user-invitations/user-invitations.component';
import { UserOrganizationsComponent } from './app/pages/user/user-organizations/user-organizations.component';
import { UserSettingsComponent } from './app/pages/user/user-settings/user-settings.component';
import { UserComponent } from './app/pages/user/user.component';
import { authGuard } from './app/core/guards/auth.guard';
import { LandingComponent } from '@features/landing/landing.component';

export const routes: Routes = [
  { path: '', redirectTo: 'landing', pathMatch: 'full' },
  {
    path: 'organization/:organizationId',
    component: OrganizationComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'members', pathMatch: 'full' },
      { path: 'members', component: MembersTableComponent },
      { path: 'invitations', component: InvitationTableComponent },
      { path: 'roles', component: RolesManagementComponent },
      { path: 'settings', component: OrganizationProfilComponent },
      { path: '**', redirectTo: 'members' },
    ],
  },
  {
    path: 'user',
    component: UserComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'organizations', pathMatch: 'full' },
      { path: 'organizations', component: UserOrganizationsComponent },
      { path: 'invitations', component: UserInvitationsComponent },
      { path: 'settings', component: UserSettingsComponent },
      { path: '**', redirectTo: 'organizations' },
    ],
  },
  {
    path: 'landing',
    component: LandingComponent,
  },
  { path: '**', redirectTo: 'landing' },
];
