import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '@core/services/auth.service';

/**
 * Guard checking if user is authenticated
 * @returns true if user is authenticated, otherwise redirects to unauthorized page
 */
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.hasValidAccessToken()) {
    return router.createUrlTree(['/unauthorized']);
  }

  return true;
};
