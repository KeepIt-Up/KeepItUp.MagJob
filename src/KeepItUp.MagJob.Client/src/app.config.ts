import { ApplicationConfig, provideAppInitializer, inject } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideOAuthClient } from 'angular-oauth2-oidc';
import { tokenInterceptor } from './app/core/interceptors/token.interceptor';
import { AuthService } from './app/core/services/auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([tokenInterceptor])),
    provideRouter(routes, withComponentInputBinding()),
    provideOAuthClient(),
    provideAppInitializer(() => {
      const authService = inject(AuthService);
      return authService.initAuth();
    }),
    provideAnimationsAsync('noop'),
  ],
};
