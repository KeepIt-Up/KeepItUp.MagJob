import { Injectable, inject } from '@angular/core';
import { AuthConfig, OAuthEvent, OAuthService, OAuthSuccessEvent } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from '@core/configs/auth.config';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly oAuthService = inject(OAuthService);

  /**
   * Redirects to the Keycloak registration page
   */
  initRegistrationFlow(): void {
    const authConfig = authCodeFlowConfig;

    // Construct the registration URL
    const registrationUrl =
      `${authConfig.issuer}/protocol/openid-connect/registrations` +
      `?client_id=${authConfig.clientId}` +
      `&redirect_uri=${encodeURIComponent(authConfig.redirectUri ?? window.location.origin)}` +
      `&response_type=${authConfig.responseType}` +
      `&scope=${authConfig.scope}`;

    // Redirect to the registration page
    window.location.href = registrationUrl;
  }

  /**
   * Redirects to the Keycloak login page
   */
  initLoginFlow(): void {
    this.oAuthService.initLoginFlow();
  }

  /**
   * Logs out the user
   */
  logOut(): void {
    this.oAuthService.logOut();
  }

  /**
   * Checks if the user has a valid access token
   * @returns true if the user has a valid access token, otherwise false
   */
  hasValidAccessToken(): boolean {
    return this.oAuthService.hasValidAccessToken();
  }

  /**
   * Gets the user's access token
   * @returns the user's access token
   */
  getAccessToken(): string {
    return this.oAuthService.getAccessToken();
  }

  /**
   * Gets the user's refresh token
   * @returns the user's refresh token
   */
  getRefreshToken(): string | null {
    return this.oAuthService.getRefreshToken();
  }

  /**
   * Gets the events observable
   * @returns the events observable
   */
  getEvents(): Observable<OAuthEvent> {
    return this.oAuthService.events;
  }

  /**
   * Configures the OAuth service
   * @param config the auth config
   */
  configure(config: AuthConfig): void {
    this.oAuthService.configure(config);
  }

  /**
   * Sets up automatic silent refresh
   */
  setupAutomaticSilentRefresh(): void {
    this.oAuthService.setupAutomaticSilentRefresh();
  }

  /**
   * Loads the discovery document
   * @returns the discovery document
   */
  loadDiscoveryDocument(): Promise<OAuthSuccessEvent> {
    return this.oAuthService.loadDiscoveryDocument();
  }

  /**
   * Tries to login
   * @returns the login result
   */
  tryLogin(): Promise<boolean> {
    return this.oAuthService.tryLogin();
  }
}
