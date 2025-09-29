import { ApplicationConfig, provideAppInitializer } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideOAuthClient } from 'angular-oauth2-oidc';
import { tokenInterceptorFn } from '@core/interceptors/token.interceptor';
import { heroIconsProvider } from '@core/providers/hero-icons-provider';
import { provideNgIconsConfig } from '@ng-icons/core';
import { appInitializerFn } from '@core/initializers/app.initializer';
import { ngIconsConfig } from '@core/configs/ng-icon.config';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([tokenInterceptorFn])),
    provideRouter(routes, withComponentInputBinding()),
    provideOAuthClient(),
    provideAppInitializer(appInitializerFn),
    provideNgIconsConfig(ngIconsConfig),
    heroIconsProvider,
  ],
};
