/**
 * Environment configuration as template for other environments
 */
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000',
  keycloakConfig: {
    url: 'http://localhost:18080',
    realm: 'keepitup-magjob',
    clientId: 'keepitup-magjob-client',
    redirectUri: `${window.location.origin}/user`,
  },
};
