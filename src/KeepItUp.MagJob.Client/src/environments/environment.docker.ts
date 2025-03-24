export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000',
  keycloakConfig: {
    url: 'http://localhost:18080',
    realm: 'magjob-realm',
    clientId: 'client.web',
    redirectUri: `${window.location.origin}/user`,
    dummyClientSecret: 'zLvcsyGzuY2BLDAhG3WGBnLxBsWxdmfd',
  },
};
