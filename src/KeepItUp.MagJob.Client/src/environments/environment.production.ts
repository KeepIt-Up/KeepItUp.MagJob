export const environment = {
  production: true,
  apiUrl: 'https://GATEWAY_URL',
  keycloakConfig: {
    url: 'https://KEYCLOAK_URL',
    realm: 'keepitup-magjob',
    clientId: 'keepitup-magjob-client',
    redirectUri: `${window.location.origin}/user`,
    dummyClientSecret: 'YOUR_DUMMY_CLIENT_SECRET',
  },
};
