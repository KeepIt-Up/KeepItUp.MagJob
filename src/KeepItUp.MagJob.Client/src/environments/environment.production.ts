export const environment = {
  production: true,
  apiUrl: 'https://GATEWAY_URL',
  keycloakConfig: {
    url: 'https://KEYCLOAK_URL',
    realm: 'magjob-realm',
    clientId: 'client.web',
    redirectUri: `${window.location.origin}/user`,
    dummyClientSecret: 'YOUR_DUMMY_CLIENT_SECRET',
  },
};
