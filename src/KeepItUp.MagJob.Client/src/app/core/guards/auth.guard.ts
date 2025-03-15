import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';

/**
 * Guard sprawdzający, czy użytkownik jest zalogowany
 * @param state - Obiekt reprezentujący bieżący stan ścieżki
 * @returns true jeśli użytkownik jest zalogowany, w przeciwnym razie przekierowuje do strony logowania
 */
export const authGuard: CanActivateFn = state => {
  const oauthService = inject(OAuthService);
  const router = inject(Router);

  if (!oauthService.hasValidAccessToken()) {
    return router.createUrlTree(['/login']);
  }

  return true;
};
