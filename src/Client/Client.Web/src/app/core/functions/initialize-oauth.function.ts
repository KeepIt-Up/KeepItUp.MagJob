import { authCodeFlowConfig } from "@core/configs/auth.config";
import { OAuthService } from "angular-oauth2-oidc";

export function initializeOAuth(oauthService: OAuthService): Promise<void> {
  return new Promise((resolve) => {
    // Konfiguracja OAuthService
    oauthService.configure(authCodeFlowConfig);

    // Jawnie ustawiamy requireHttps na false w środowisku deweloperskim
    if (!authCodeFlowConfig.requireHttps) {
      oauthService.requireHttps = false;
    }

    // Włączenie automatycznego odświeżania tokenów
    oauthService.setupAutomaticSilentRefresh();

    // Dodajemy obsługę zdarzeń
    oauthService.events.subscribe(event => {
      console.log('OAuth event:', event);

      // Obsługa błędów
      if (event.type === 'token_error' || event.type === 'token_refresh_error') {
        console.error('Token error:', event);
      }

      // Obsługa otrzymania tokenu
      if (event.type === 'token_received') {
        console.log('Token received');
        const claims = oauthService.getIdentityClaims();
        if (claims) {
          console.log('User claims:', claims);
        }
      }
    });

    // Ładowanie dokumentu discovery i próba logowania
    oauthService.loadDiscoveryDocument()
      .then(() => {
        console.log('Discovery document loaded');

        // Próba logowania
        return oauthService.tryLogin();
      })
      .then(() => {
        console.log('Login attempt completed');

        // Sprawdzenie, czy użytkownik jest zalogowany
        if (oauthService.hasValidAccessToken()) {
          console.log('User is logged in');
        } else {
          console.log('User is not logged in');
        }

        resolve();
      })
      .catch(error => {
        console.error('Error during OAuth initialization:', error);
        // Rozwiązujemy obietnicę mimo błędu, aby aplikacja mogła się uruchomić
        resolve();
      });
  });
}