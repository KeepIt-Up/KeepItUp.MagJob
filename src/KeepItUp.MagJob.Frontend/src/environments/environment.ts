import packageJson from '../../package.json';

export const environment = {
  production: false,
  mockUser: false,
  appVersion: packageJson.version,
  apiUrl: 'http://localhost:5000',
  keycloakConfig: {
    url: 'http://localhost:18080',
    realm: 'keepitup-magjob',
    clientId: 'keepitup-magjob-client',
    redirectUri: `${window.location.origin}/user`,
  },
};
