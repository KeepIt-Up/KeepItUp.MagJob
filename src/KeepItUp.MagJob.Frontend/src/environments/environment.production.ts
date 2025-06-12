import packageJson from '../../package.json';

export const environment = {
  production: true,
  mockUser: false,
  appVersion: packageJson.version,
  apiUrl: 'https://GATEWAY_URL',
  keycloakConfig: {
    url: 'https://KEYCLOAK_URL',
    realm: 'keepitup-magjob',
    clientId: 'keepitup-magjob-client',
    redirectUri: `${window.location.origin}/user`,
    dummyClientSecret: 'YOUR_DUMMY_CLIENT_SECRET',
  },
};
