import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-landing',
  imports: [RouterLink, NavbarComponent],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss',
})
export class LandingComponent implements OnInit {
  errorMessage: string | null = null;
  returnUrl = '/dashboard';

  constructor(
    private oauthService: OAuthService,
    private route: ActivatedRoute,
    private router: Router,
  ) {}

  ngOnInit(): void {
    // Sprawdź, czy użytkownik jest już zalogowany
    if (this.oauthService.hasValidAccessToken()) {
      console.log('User already logged in, redirecting to dashboard');
      void this.router.navigate([this.returnUrl]);
      return;
    }

    // Pobierz returnUrl z parametrów zapytania
    this.route.queryParams.subscribe(params => {
      if (params['returnUrl']) {
        this.returnUrl = params['returnUrl'] as string;
      }
    });

    // Sprawdź, czy jest komunikat o błędzie
    this.route.queryParams.subscribe(params => {
      if (params['error']) {
        this.errorMessage = params['error'] as string;
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
        void this.router.navigate([this.returnUrl]);
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
