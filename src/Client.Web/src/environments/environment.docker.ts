import { AuthConfig } from "angular-oauth2-oidc";

export const environment = {
  apiUrl: 'https://localhost:5001/api',
  keycloakConfig: {
    issuer: 'http://localhost:18080/realms/magjob-realm',
    tokenEndpoint: 'http://localhost:18080/realms/magjob-realm/protocol/openid-connect/token',
    redirectUri: window.location.origin,
    clientId: 'web-client',
    responseType: 'code',
    scope: 'openid profile',
  } as AuthConfig
};
