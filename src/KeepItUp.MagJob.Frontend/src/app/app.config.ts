import { ApplicationConfig, LOCALE_ID, provideAppInitializer } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { registerLocaleData } from '@angular/common';
import localePl from '@angular/common/locales/pl';

import { routes } from './app.routes';
import { provideOAuthClient } from 'angular-oauth2-oidc';
import { appInitializerFn } from '@core/initializers/app.initializer';
import { MessageService } from 'primeng/api';
import { tokenInterceptorFn } from '@core/interceptors/token.interceptor';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';

// Register Polish locale
registerLocaleData(localePl);

export const appConfig: ApplicationConfig = {
  providers: [
    { provide: LOCALE_ID, useValue: 'pl' },
    provideHttpClient(withInterceptorsFromDi(), withInterceptors([tokenInterceptorFn])),
    provideAnimationsAsync(),
    provideRouter(routes, withComponentInputBinding()),
    provideOAuthClient(),
    provideAppInitializer(appInitializerFn),
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          prefix: 'p',
          darkModeSelector: '.dark',
        },
      },
    }),
    MessageService,
  ],
};
