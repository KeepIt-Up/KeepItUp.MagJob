import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '../../../environments/environment';

export const authCodeFlowConfig: AuthConfig = {
  issuer: `${environment.keycloakConfig.url}/realms/${environment.keycloakConfig.realm}`,
  loginUrl: `${environment.keycloakConfig.url}/realms/${environment.keycloakConfig.realm}/protocol/openid-connect/auth`,
  tokenEndpoint: `${environment.keycloakConfig.url}/realms/${environment.keycloakConfig.realm}/protocol/openid-connect/token`,
  userinfoEndpoint: `${environment.keycloakConfig.url}/realms/${environment.keycloakConfig.realm}/protocol/openid-connect/userinfo`,
  redirectUri: window.location.origin + '/user',
  clientId: environment.keycloakConfig.clientId,
  responseType: 'code',
  scope: 'openid profile email',
  logoutUrl: `${environment.keycloakConfig.url}/realms/${environment.keycloakConfig.realm}/protocol/openid-connect/logout`,
  showDebugInformation: true,
  requireHttps: false,
  disableAtHashCheck: true,
  oidc: true,
  useHttpBasicAuth: true,
  useSilentRefresh: true,
  silentRefreshTimeout: 5000,
  timeoutFactor: 0.75,
  sessionChecksEnabled: true,
  clearHashAfterLogin: true,
  nonceStateSeparator: 'semicolon',
  skipIssuerCheck: true,
};
