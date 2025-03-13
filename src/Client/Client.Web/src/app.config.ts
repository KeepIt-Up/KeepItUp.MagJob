import { ApplicationConfig, provideAppInitializer, inject } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { OAuthService, provideOAuthClient } from 'angular-oauth2-oidc';
import { tokenInterceptor } from './app/core/interceptors/token.interceptor';
import { initializeOAuth } from '@core/functions/initialize-oauth.function';


//TODO: ADD comments with explanation
export const appConfig: ApplicationConfig = {
    providers: [
        provideHttpClient(
            withInterceptors([tokenInterceptor])
        ),
        provideRouter(routes, withComponentInputBinding()),
        provideOAuthClient(),
        provideAppInitializer(async () => {
            const oauthService = inject(OAuthService);
            await initializeOAuth(oauthService);
        }),
        provideAnimationsAsync('noop'),
    ],
};
