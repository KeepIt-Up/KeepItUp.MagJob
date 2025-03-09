import { AuthConfig } from "angular-oauth2-oidc";

export const environment = {
  apiUrl: '',
  keycloakConfig: {
    issuer: '',
    tokenEndpoint: '',
    redirectUri: window.location.origin,
    clientId: 'client.w eb',
    responseType: 'code',
    scope: 'openid profile',
  } as AuthConfig
};
