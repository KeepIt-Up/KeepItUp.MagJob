import { ApplicationConfig, provideAppInitializer, inject, LOCALE_ID } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideOAuthClient } from 'angular-oauth2-oidc';
import { tokenInterceptorFn } from '@core/interceptors/token.interceptor';
import { heroIconsProvider } from '@core/providers/hero-icons-provider';
import { provideNgIconsConfig } from '@ng-icons/core';
import { appInitializerFn } from '@core/initializers/app.initializer';
import { ngIconsConfig } from '@core/configs/ng-icon.config';

import {
  CalendarDateFormatter,
  CalendarNativeDateFormatter,
  DateAdapter,
  CalendarUtils,
  CalendarA11y,
  CalendarEventTitleFormatter,
} from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/moment';
import moment from 'moment';

export function momentAdapterFactory() {
  return adapterFactory(moment);
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([tokenInterceptorFn])),
    provideRouter(routes, withComponentInputBinding()),
    provideOAuthClient(),
    provideAppInitializer(appInitializerFn),
    provideNgIconsConfig(ngIconsConfig),
    heroIconsProvider,
    { provide: LOCALE_ID, useValue: 'en-US' },
    { provide: DateAdapter, useFactory: momentAdapterFactory },
    { provide: CalendarDateFormatter, useClass: CalendarNativeDateFormatter },
    CalendarUtils,
    CalendarA11y,
    CalendarEventTitleFormatter,
  ],
};
