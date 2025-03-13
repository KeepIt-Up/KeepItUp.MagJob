import { AuthConfig } from "angular-oauth2-oidc";

/**
 * Environment configuration as template for other environments
 */
export const environment = {
  production: false,
  apiUrl: 'http://GATEWAY_URL',
  keycloakConfig: {
    issuer: 'http://KEYCLOAK_URL/realms/magjob-realm',
    loginUrl: 'http://KEYCLOAK_URL/realms/magjob-realm/protocol/openid-connect/auth',
    tokenEndpoint: 'http://KEYCLOAK_URL/realms/magjob-realm/protocol/openid-connect/token',
    userinfoEndpoint: 'http://KEYCLOAK_URL/realms/magjob-realm/protocol/openid-connect/userinfo',
    redirectUri: window.location.origin,
    clientId: 'client.web',
    responseType: 'code',
    scope: 'openid profile email',
    logoutUrl: 'http://KEYCLOAK_URL/realms/magjob-realm/protocol/openid-connect/logout',
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
    dummyClientSecret: ''
  } as AuthConfig
};
