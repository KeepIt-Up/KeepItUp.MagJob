import { Routes } from '@angular/router';
import { UserDashboardComponent } from './components/user-dashboard/user-dashboard.component';

export const USER_ROUTES: Routes = [
  {
    path: '',
    component: UserDashboardComponent,
  },
];
