import { Injectable, inject } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';
import { User } from '@features/models/user/user';
import { authCodeFlowConfig } from '@core/configs/auth.config';

interface IdentityClaims {
  sub: string;
  given_name?: string;
  family_name?: string;
  email?: string;
  [key: string]: unknown;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private userProfileSubject = new BehaviorSubject<User | null>(null);

  private readonly oauthService = inject(OAuthService);
  private readonly router = inject(Router);

  /**
   * Inicjalizuje uwierzytelnianie
   * @returns Promise reprezentujący asynchroniczną operację
   */
  public async initAuth(): Promise<void> {
    return new Promise(resolve => {
      // Konfiguracja OAuthService
      this.oauthService.configure(authCodeFlowConfig);

      // Włączenie automatycznego odświeżania tokenów
      this.oauthService.setupAutomaticSilentRefresh();

      // Konfiguracja obsługi zdarzeń
      this.setupEventHandlers();

      // Ładowanie dokumentu discovery i próba logowania
      this.oauthService
        .loadDiscoveryDocument()
        .then(() => {
          return this.oauthService.tryLogin();
        })
        .then(() => {
          this.loadUserProfile();
          resolve();
        })
        .catch(error => {
          console.error('Error during OAuth initialization:', error);
          resolve();
        });
    });
  }

  /**
   * Inicjuje proces logowania
   * @param redirectUri Opcjonalny URI przekierowania po zalogowaniu
   */
  public login(redirectUri?: string): void {
    this.oauthService.initLoginFlow(redirectUri);
  }

  /**
   * Wylogowuje użytkownika
   */
  public logout(): void {
    this.oauthService.logOut();
  }

  /**
   * Sprawdza, czy użytkownik jest uwierzytelniony
   * @returns Czy użytkownik jest uwierzytelniony
   */
  public isAuthenticated(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  /**
   * Pobiera token dostępu
   * @returns Token dostępu
   */
  public getAccessToken(): string {
    return this.oauthService.getAccessToken();
  }

  /**
   * Pobiera identyfikator użytkownika
   * @returns Identyfikator użytkownika
   */
  public getUserId(): string | null {
    const claims = this.oauthService.getIdentityClaims() as IdentityClaims;
    return claims ? claims.sub : null;
  }

  /**
   * Pobiera profil użytkownika
   * @returns Observable z profilem użytkownika
   */
  public getUserProfile(): Observable<User | null> {
    return this.userProfileSubject.asObservable();
  }

  /**
   * Ładuje profil użytkownika
   */
  private loadUserProfile(): void {
    const claims = this.oauthService.getIdentityClaims() as IdentityClaims;
    if (claims) {
      const user: User = {
        id: claims.sub,
        firstname: claims.given_name ?? '',
        lastname: claims.family_name ?? '',
        email: claims.email ?? '',
      };
      this.userProfileSubject.next(user);
    }
  }

  /**
   * Konfiguruje obsługę zdarzeń OAuth
   */
  private setupEventHandlers(): void {
    this.oauthService.events.subscribe(event => {
      if (event.type === 'token_received') {
        this.loadUserProfile();
      } else if (event.type === 'logout') {
        this.userProfileSubject.next(null);
        void this.router.navigate(['/landing']);
      } else if (event.type === 'token_error' || event.type === 'token_refresh_error') {
        console.error('Token error:', event);
      }
    });
  }
}
