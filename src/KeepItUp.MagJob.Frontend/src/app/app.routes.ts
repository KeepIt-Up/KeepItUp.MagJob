import { Routes } from '@angular/router';
import { ItemsListComponent } from '@features/items/components/items-list/items-list.component';
import { ItemsInfiniteListComponent } from '@features/items/components/items-infinite-list/items-infinite-list.component';
import { ItemFormComponent } from '@features/items/components/item-form/item-form.component';
import { AnonymousLayoutComponent } from '@features/anonymous/components/anonymous-layout/anonymous-layout.component';
import { LandingComponent } from '@features/anonymous/components/landing/landing.component';
import { UnauthorizedComponent } from '@features/anonymous/components/unauthorized/unauthorized.component';
import { HelpComponent } from '@features/anonymous/components/help/help.component';
import { NotFoundComponent } from '@features/anonymous/components/not-found/not-found.component';
import { AuthenticatedLayoutComponent } from '@features/authenticated/components/authenticated-layout/authenticated-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: AnonymousLayoutComponent,
    children: [
      {
        path: '',
        component: LandingComponent,
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
    ],
  },
  {
    path: 'organization',
    component: AuthenticatedLayoutComponent,
    loadChildren: () =>
      import('@features/authenticated/components/organization-platform/organization.routes').then(
        m => m.ORGANIZATION_ROUTES,
      ),
  },
  {
    path: 'user',
    component: AuthenticatedLayoutComponent,
    loadChildren: () =>
      import('@features/authenticated/components/user-platform/user.routes').then(
        m => m.USER_ROUTES,
      ),
  },
  {
    path: 'items',
    component: ItemsListComponent,
  },
  {
    path: 'items/add',
    component: ItemFormComponent,
  },
  {
    path: 'items/edit/:itemId',
    component: ItemFormComponent,
  },
  {
    path: 'items/infinite',
    component: ItemsInfiniteListComponent,
  },
  {
    path: '**',
    redirectTo: '404',
  },
];
