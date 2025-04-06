import { ApplicationConfig, provideAppInitializer, inject, LOCALE_ID } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideOAuthClient } from 'angular-oauth2-oidc';
import { tokenInterceptor } from './app/core/interceptors/token.interceptor';
import { AuthService } from './app/core/services/auth.service';
import { heroIcons } from './app/shared/icons/icons';
import { provideNgIconsConfig } from '@ng-icons/core';
import { 
  CalendarDateFormatter, 
  CalendarNativeDateFormatter, 
  DateAdapter, 
  CalendarUtils,
  CalendarA11y,
  CalendarEventTitleFormatter 
} from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/moment';
import moment from 'moment';

export function momentAdapterFactory() {
  return adapterFactory(moment);
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([tokenInterceptor])),
    provideRouter(routes, withComponentInputBinding()),
    provideOAuthClient(),
    provideAppInitializer(() => {
      const authService = inject(AuthService);
      return authService.initAuth();
    }),
    provideNgIconsConfig({
      size: '1.5em',
      color: 'currentColor',
      strokeWidth: 2,
    }),
    heroIcons,
    { provide: LOCALE_ID, useValue: 'en-US' },
    { provide: DateAdapter, useFactory: momentAdapterFactory },
    { provide: CalendarDateFormatter, useClass: CalendarNativeDateFormatter },
    CalendarUtils,
    CalendarA11y,
    CalendarEventTitleFormatter,
  ],
};
