import { authCodeFlowConfig } from '../configs/auth.config';
import { AuthService } from '@core/services/auth.service';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { UserContextService } from '@users/services/user-context.service';

export const appInitializerFn = (): Observable<unknown> | Promise<unknown> | void => {
  const authService = inject(AuthService);
  const userContextService = inject(UserContextService);

  return new Promise(resolve => {
    // Configure OAuthService
    authService.configure(authCodeFlowConfig);

    // Enable automatic token refresh
    authService.setupAutomaticSilentRefresh();

    // Load discovery document and try login
    authService
      .loadDiscoveryDocument()
      .then(() => {
        return authService.tryLogin();
      })
      .then(() => {
        userContextService.loadCurrentUser().subscribe();
        resolve(void 0);
      })
      .catch(error => {
        console.error('Error during OAuth initialization:', error);
        resolve(void 0);
      });
  });
};
