import { authCodeFlowConfig } from '../configs/auth.config';
import { inject } from '@angular/core';
import { AuthService } from '@core/auth/services/auth.service';
import { Observable } from 'rxjs';

export const appInitializerFn = (): Observable<unknown> | Promise<unknown> | void => {
  const authService = inject(AuthService);

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
        // TOOD: load user context
        resolve(void 0);
      })
      .catch(error => {
        console.error('Error during OAuth initialization:', error);
        resolve(void 0);
      });
  });
};
