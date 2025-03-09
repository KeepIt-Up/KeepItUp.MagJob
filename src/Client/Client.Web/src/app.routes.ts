import { Routes } from '@angular/router';
import { CalendarComponent } from '@features/calendar/calendar.component';
import { InvitationTableComponent } from '@pages/organization/invitation-table/invitation-table.component';
import { MembersTableComponent } from '@pages/organization/members-table/member-table.component';
import { OrganizationProfilComponent } from '@pages/organization/organization-profil/organization-profil.component';
import { OrganizationComponent } from '@pages/organization/organization.component';
import { RolesManagementComponent } from '@pages/organization/roles-management/roles-management.component';
import { OrganizationCreatorComponent } from '@pages/public/organization-creator/organization-creator.component';
import { PublicComponent } from '@pages/public/public.component';
import { UserInvitationsComponent } from '@pages/user/user-invitations/user-invitations.component';
import { UserOrganizationsComponent } from '@pages/user/user-organizations/user-organizations.component';
import { UserSettingsComponent } from '@pages/user/user-settings/user-settings.component';
import { UserComponent } from '@pages/user/user.component';

export const routes: Routes = [

  { path: '', redirectTo: 'user', pathMatch: 'full' },
  {
    path: 'organization/:organizationId', component: OrganizationComponent, children: [
      { path: '', redirectTo: 'members', pathMatch: 'full' },
      { path: 'members', component: MembersTableComponent },
      { path: 'invitations', component: InvitationTableComponent },
      { path: 'roles', component: RolesManagementComponent },

      { path: 'settings', component: OrganizationProfilComponent },
      { path: '**', redirectTo: 'members' }
    ]
  },
  {
    path: 'user', component: UserComponent, children: [
      { path: '', redirectTo: 'organizations', pathMatch: 'full' },
      { path: 'organizations', component: UserOrganizationsComponent },
      { path: 'invitations', component: UserInvitationsComponent },
      { path: 'settings', component: UserSettingsComponent },
      { path: '**', redirectTo: 'organizations' },
    ]
  },
  {
    path: 'public', component: PublicComponent, children: [
      { path: '', redirectTo: 'create-organization', pathMatch: 'full' },
      { path: 'create-organization', component: OrganizationCreatorComponent },
      { path: '**', redirectTo: 'create-organization' }
    ]
  },
  { path: '**', redirectTo: 'public' }

];
