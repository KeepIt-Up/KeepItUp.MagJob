import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const oauthService = inject(OAuthService);
  const authService = inject(AuthService);
  const router = inject(Router);

  // Sprawdź, czy użytkownik jest zalogowany
  if (!oauthService.hasValidAccessToken()) {
    // Przekieruj do strony logowania z parametrem returnUrl
    return router.createUrlTree(['/login'], {
      queryParams: { returnUrl: state.url }
    });
  }

  // Sprawdź, czy wymagane są uprawnienia dla tej trasy
  const requiredPermissions = route.data?.['permissions'] as string[];

  if (requiredPermissions && requiredPermissions.length > 0) {
    // Sprawdź, czy użytkownik ma wszystkie wymagane uprawnienia
    const hasAllPermissions = requiredPermissions.every(permission =>
      authService.hasPermission(permission)
    );

    if (!hasAllPermissions) {
      // Przekieruj do strony z błędem 403 (brak uprawnień)
      return router.createUrlTree(['/forbidden']);
    }
  }

  // Użytkownik jest zalogowany i ma wymagane uprawnienia
  return true;
};
