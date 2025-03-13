import { Injectable, inject } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  private userProfileSubject = new BehaviorSubject<any>(null);
  private router = inject(Router);

  constructor(private oauthService: OAuthService) {
    this.setupEventHandlers();
  }

  /**
   * Konfiguruje obsługę zdarzeń OAuth
   */
  private setupEventHandlers(): void {
    this.oauthService.events.subscribe(event => {
      if (event.type === 'token_received') {
        this.isAuthenticatedSubject.next(this.oauthService.hasValidAccessToken());
        this.loadUserProfile();
      } else if (event.type === 'token_expires') {
        console.log('Token wygasł, odświeżanie...');
      } else if (event.type === 'logout') {
        this.isAuthenticatedSubject.next(false);
        this.userProfileSubject.next(null);
        this.router.navigate(['/login']);
      }
    });
  }

  /**
   * Inicjalizuje uwierzytelnianie
   * @returns Promise reprezentujący asynchroniczną operację
   */
  public async initAuth(): Promise<void> {
    try {
      await this.oauthService.loadDiscoveryDocumentAndTryLogin();
      this.isAuthenticatedSubject.next(this.oauthService.hasValidAccessToken());

      if (this.isAuthenticated()) {
        this.loadUserProfile();
      }
    } catch (error) {
      console.error('Błąd podczas inicjalizacji uwierzytelniania', error);
      throw error;
    }
  }

  /**
   * Inicjuje proces logowania
   * @param redirectUri Opcjonalny URI przekierowania po zalogowaniu
   */
  login(redirectUri?: string): void {
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
    const claims = this.oauthService.getIdentityClaims() as any;
    return claims ? claims.sub : null;
  }

  /**
   * Pobiera profil użytkownika
   * @returns Observable z profilem użytkownika
   */
  public getUserProfile(): Observable<any> {
    return this.userProfileSubject.asObservable();
  }

  /**
   * Pobiera status uwierzytelnienia
   * @returns Observable ze statusem uwierzytelnienia
   */
  public getAuthStatus(): Observable<boolean> {
    return this.isAuthenticatedSubject.asObservable();
  }

  /**
   * Pobiera uprawnienia użytkownika
   * @returns Lista uprawnień użytkownika
   */
  public getUserPermissions(): string[] {
    const claims = this.oauthService.getIdentityClaims() as any;
    return claims && claims.permissions ? claims.permissions : [];
  }

  /**
   * Sprawdza, czy użytkownik ma określone uprawnienie
   * @param permission Uprawnienie do sprawdzenia
   * @returns Czy użytkownik ma uprawnienie
   */
  public hasPermission(permission: string): boolean {
    const permissions = this.getUserPermissions();
    return permissions.includes(permission);
  }

  /**
   * Ładuje profil użytkownika
   */
  private loadUserProfile(): void {
    // Najpierw próbujemy pobrać profil z tokenu
    const claims = this.oauthService.getIdentityClaims();
    if (claims) {
      this.userProfileSubject.next(claims);
    }
  }
}
