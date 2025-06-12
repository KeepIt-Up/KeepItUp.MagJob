import { computed, inject, Injectable, signal } from '@angular/core';
import {
  BehaviorSubject,
  Observable,
  distinctUntilChanged,
  map,
  shareReplay,
  tap,
  of,
  catchError,
} from 'rxjs';
import { CurrentUser } from '../models/current-user.model';
import { UserApiService } from './user.api';
import { Router } from '@angular/router';
import { AuthService } from '@core/services/auth.service';
import { OAuthEvent } from 'angular-oauth2-oidc';
import { errorState, initialState, loadedState, loadingState, State } from '@shared/state';

@Injectable({
  providedIn: 'root',
})
export class UserContextService {
  private readonly userContextState =
    signal<State<CurrentUser | undefined>>(initialState<CurrentUser | undefined>());

  private readonly userApiService = inject(UserApiService);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  readonly $userContext = this.userContextState.asReadonly();

  constructor() {
    this.authService.getEvents().subscribe((event: OAuthEvent) => {
      if (event.type === 'token_received') {
        this.loadCurrentUser().subscribe();
      } else if (event.type === 'logout') {
        this.resetUserContext();
        void this.router.navigate(['/landing']);
      } else if (event.type === 'token_error' || event.type === 'token_refresh_error') {
        console.error('Token error:', event);
      }
    });
  }

  getCurrentUser(): CurrentUser | undefined {
    return this.userContextState().data;
  }

  loadCurrentUser(): Observable<CurrentUser | null> {
    if (!this.authService.hasValidAccessToken()) {
      const error = 'User is not authenticated';
      this.setError(error);
      return of(null);
    }

    this.setLoading();

    return this.userApiService.getCurrentUser().pipe(
      tap({
        next: user => {
          this.userContextState.set(loadedState(user));
        },
        error: (error: unknown) => {
          const errorMessage = error instanceof Error ? error.message : 'Failed to load user data';
          this.setError(errorMessage);
        },
      }),
      catchError(error => {
        const errorMessage = error instanceof Error ? error.message : 'Failed to load user data';
        this.setError(errorMessage);
        return of(null);
      }),
    );
  }

  // Set loading state
  private setLoading(): void {
    const currentUser = this.getCurrentUser();
    this.userContextState.set(loadingState(this.userContextState()));
  }

  // Set error state
  private setError(error: string): void {
    this.userContextState.set(errorState(error));
  }

  // Reset user context
  resetUserContext(): void {
    this.userContextState.set(initialState<CurrentUser | undefined>());
  }

  // Update user context
  updateUserContext(user: CurrentUser): void {
    this.userContextState.set(loadedState(user));
  }
}
