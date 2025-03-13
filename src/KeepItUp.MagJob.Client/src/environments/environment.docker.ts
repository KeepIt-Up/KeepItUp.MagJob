export const environment = {
  production: false,
  //Adjust Docker Gateway URL
  apiUrl: 'http://localhost:5000/api',
  keycloakConfig: {
    url: 'http://localhost:18080',
    realm: 'magjob-realm',
    clientId: 'client.web',
    redirectUri: `${window.location.origin}/user`,
    dummyClientSecret: 'bYBrriEeDclOCaDTVneVAbeCrbgnWrWd',
  },
};
