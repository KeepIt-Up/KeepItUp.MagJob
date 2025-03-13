import { AuthConfig } from 'angular-oauth2-oidc';

export const environment = {
  production: true,
  apiUrl: 'https://GATEWAY_PRODUCTION_URL',
  keycloakConfig: {
    issuer: 'https://KEYCLOAK_PRODUCTION_URL/realms/magjob-realm',
    loginUrl: 'https://KEYCLOAK_PRODUCTION_URL/realms/magjob-realm/protocol/openid-connect/auth',
    tokenEndpoint:
      'https://KEYCLOAK_PRODUCTION_URL/realms/magjob-realm/protocol/openid-connect/token',
    userinfoEndpoint:
      'https://KEYCLOAK_PRODUCTION_URL/realms/magjob-realm/protocol/openid-connect/userinfo',
    redirectUri: window.location.origin,
    clientId: 'client.web',
    responseType: 'code',
    scope: 'openid profile email',
    logoutUrl: 'https://KEYCLOAK_PRODUCTION_URL/realms/magjob-realm/protocol/openid-connect/logout',
    showDebugInformation: false,
    requireHttps: true,
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
    dummyClientSecret: '',
  } as AuthConfig,
};
