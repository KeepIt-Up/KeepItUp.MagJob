import { Routes } from '@angular/router';
import { LandingComponent } from '@pages/landing/landing.component';
import { HelpComponent } from '@pages/help/help.component';
import { UserComponent } from './app/pages/user/user.component';
import { UserOrganizationsComponent } from './app/pages/user/user-organizations/user-organizations.component';
import { UserInvitationsComponent } from './app/pages/user/user-invitations/user-invitations.component';
import { UserSettingsComponent } from './app/pages/user/user-settings/user-settings.component';
import { authGuard } from '@core/guards/auth.guard';
import { OrganizationProfilComponent } from '@pages/organization/organization-profil/organization-profil.component';
import { RolesManagementComponent } from '@pages/organization/roles-management/roles-management.component';
import { MembersTableComponent } from '@pages/organization/members-table/member-table.component';
import { InvitationTableComponent } from '@pages/organization/invitation-table/invitation-table.component';
import { OrganizationComponent } from '@pages/organization/organization.component';
import { CreateOrganizationComponent } from '@pages/organization/create-organization/create-organization.component';
import { UnauthorizedComponent } from '@pages/unauthorized/unauthorized.component';
import { NotFoundComponent } from '@pages/not-found/not-found.component';
import { WorkEvidenceComponent } from './app/features/workevidence/workevidence.component';
import { ShiftComponent } from './app/features/shift/shift.component';
import { CalendarPageComponent } from './app/pages/calendar-page';
import { AvailabilityTemplatesComponent } from './app/pages/availability-templates/availability-templates.component';
import { GraphicsComponent } from './app/pages/user/graphics/graphics.component';

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
      {
        path: 'graphics',
        component: GraphicsComponent,
      },
      {
        path: 'graphics/:id',
        loadComponent: () =>
          import('./app/pages/user/graphic-detail/graphic-detail.component').then(
            m => m.GraphicDetailComponent,
          ),
      },
      {
        path: 'graphics/:id/manage-members',
        loadComponent: () =>
          import('./app/pages/user/graphic-manage-members/graphic-manage-members.component').then(
            m => m.GraphicManageMembersComponent,
          ),
      },
      { path: 'myCalendar', component: CalendarPageComponent, data: { viewMode: 'employee' } },
      { path: 'organizations', component: UserOrganizationsComponent },
      { path: 'invitations', component: UserInvitationsComponent },
      { path: 'settings', component: UserSettingsComponent },
      { path: 'calendar', component: CalendarPageComponent, data: { viewMode: 'manager' } },
      { path: 'workevidence', component: WorkEvidenceComponent },
      { path: 'shift', component: ShiftComponent },
      { path: 'availability-templates', component: AvailabilityTemplatesComponent },
      { path: '**', redirectTo: 'organizations' },
    ],
  },
  {
    path: 'landing',
    component: LandingComponent,
  },
  {
    path: 'create-organization',
    component: CreateOrganizationComponent,
    canActivate: [authGuard],
  },
  {
    path: 'help',
    component: HelpComponent,
  },
  {
    path: 'unauthorized',
    component: UnauthorizedComponent,
  },
  {
    path: '404',
    component: NotFoundComponent,
  },
  { path: '**', redirectTo: '404' },
];
