import { AuthConfig } from 'angular-oauth2-oidc';

/**
 * Environment configuration for local development
 * Copy this file to environment.local.ts and fill in the values
 * @see environment.ts
 */
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  keycloakConfig: {
    issuer: 'http://localhost:18080/realms/magjob-realm',
    loginUrl: 'http://localhost:18080/realms/magjob-realm/protocol/openid-connect/auth',
    tokenEndpoint: 'http://localhost:18080/realms/magjob-realm/protocol/openid-connect/token',
    userinfoEndpoint: 'http://localhost:18080/realms/magjob-realm/protocol/openid-connect/userinfo',
    redirectUri: window.location.origin + '/user',
    clientId: 'client.web',
    responseType: 'code',
    scope: 'openid profile email',
    logoutUrl: 'http://localhost:18080/realms/magjob-realm/protocol/openid-connect/logout',
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
    dummyClientSecret: 'bYBrriEeDclOCaDTVneVAbeCrbgnWrWd',
  } as AuthConfig,
};
