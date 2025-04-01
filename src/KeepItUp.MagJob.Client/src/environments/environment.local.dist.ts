/**
 * Environment configuration for local development
 * Copy this file to environment.local.ts and fill in the values
 * @see environment.ts
 */
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000',
  keycloakConfig: {
    url: 'http://localhost:18080',
    realm: 'magjob-realm',
    clientId: 'client.web',
    redirectUri: `${window.location.origin}/user`,
  },
};
