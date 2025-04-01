/**
 * Environment configuration as template for other environments
 */
export const environment = {
  production: false,
  apiUrl: 'http://localhost:18080',
  keycloakConfig: {
    url: 'http://localhost:18080',
    realm: 'KeepItUp.MagJob.Realm',
    clientId: 'Client-keycloakClient',
    redirectUri: `${window.location.origin}/user`,
  },
};
