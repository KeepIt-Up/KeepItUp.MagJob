/**
 * Environment configuration as template for other environments
 */
export const environment = {
  production: false,
  apiUrl: 'http://GATEWAY_URL',
  keycloakConfig: {
    url: 'https://KEYCLOAK_URL',
    realm: 'magjob-realm',
    clientId: 'client.web',
    redirectUri: `${window.location.origin}/user`,
  },
};
