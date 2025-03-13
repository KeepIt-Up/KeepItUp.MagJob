import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ActivatedRoute, Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <h1>MagJob 2.0</h1>
          <p>Zaloguj się, aby kontynuować</p>
        </div>

        <div class="login-content">
          <button (click)="onLogin()" class="login-button">
            <span class="button-icon">🔐</span>
            <span class="button-text">Zaloguj się przez Keycloak</span>
          </button>

          <div *ngIf="errorMessage" class="error-message">
            {{ errorMessage }}
          </div>
        </div>

        <div class="login-footer">
          <p>© 2023 MagJob 2.0. Wszelkie prawa zastrzeżone.</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      background-color: #f5f5f5;
    }

    .login-card {
      width: 100%;
      max-width: 400px;
      background-color: white;
      border-radius: 8px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
      overflow: hidden;
    }

    .login-header {
      padding: 2rem;
      text-align: center;
      background-color: #3498db;
      color: white;
    }

    .login-header h1 {
      margin: 0;
      font-size: 2rem;
      font-weight: 700;
    }

    .login-header p {
      margin: 0.5rem 0 0;
      opacity: 0.9;
    }

    .login-content {
      padding: 2rem;
    }

    .login-button {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 100%;
      padding: 0.75rem 1rem;
      background-color: #2ecc71;
      color: white;
      border: none;
      border-radius: 4px;
      font-size: 1rem;
      cursor: pointer;
      transition: background-color 0.3s;
    }

    .login-button:hover {
      background-color: #27ae60;
    }

    .button-icon {
      margin-right: 0.5rem;
      font-size: 1.25rem;
    }

    .error-message {
      margin-top: 1rem;
      padding: 0.75rem;
      background-color: #ffebee;
      color: #c62828;
      border-radius: 4px;
      text-align: center;
    }

    .login-footer {
      padding: 1rem;
      text-align: center;
      font-size: 0.8rem;
      color: #7f8c8d;
      border-top: 1px solid #f5f5f5;
    }
  `]
})
export class LoginComponent implements OnInit {
  errorMessage: string | null = null;
  returnUrl: string = '/dashboard';

  constructor(
    private oauthService: OAuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Sprawdź, czy użytkownik jest już zalogowany
    if (this.oauthService.hasValidAccessToken()) {
      console.log('User already logged in, redirecting to dashboard');
      this.router.navigate([this.returnUrl]);
      return;
    }

    // Pobierz returnUrl z parametrów zapytania
    this.route.queryParams.subscribe(params => {
      if (params['returnUrl']) {
        this.returnUrl = params['returnUrl'];
      }
    });

    // Sprawdź, czy jest komunikat o błędzie
    this.route.queryParams.subscribe(params => {
      if (params['error']) {
        this.errorMessage = params['error'];
      }
    });

    // Nasłuchuj zdarzeń OAuth
    this.oauthService.events.subscribe(event => {
      if (event.type === 'token_error' || event.type === 'token_refresh_error') {
        console.error('Token error in login component:', event);
        this.errorMessage = 'Wystąpił błąd podczas logowania. Spróbuj ponownie.';
      }

      if (event.type === 'token_received') {
        console.log('Token received in login component, redirecting to dashboard');
        this.router.navigate([this.returnUrl]);
      }
    });
  }

  onLogin(): void {
    console.log('Initiating login flow');
    try {
      // Wyczyść ewentualne poprzednie błędy
      this.errorMessage = null;

      // Inicjuj proces logowania
      this.oauthService.initLoginFlow(this.returnUrl);
    } catch (error) {
      console.error('Error during login initiation:', error);
      this.errorMessage = 'Wystąpił błąd podczas inicjowania logowania. Spróbuj ponownie.';
    }
  }
}
