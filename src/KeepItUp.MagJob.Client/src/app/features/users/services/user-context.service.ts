import { inject, Injectable } from '@angular/core';
import {
  BehaviorSubject,
  Observable,
  distinctUntilChanged,
  map,
  shareReplay,
  tap,
  of,
  catchError,
  first,
} from 'rxjs';
import { CurrentUser } from '../models/current-user.model';
import {
  DataState,
  createInitialState,
  createLoadingState,
  createSuccessState,
  createErrorState,
} from '../../../shared/data-state/data-state.model';
import { UserApiService } from './user.api';
import { Router } from '@angular/router';
import { AuthService } from '@core/services/auth.service';
import { OAuthEvent } from 'angular-oauth2-oidc';

@Injectable({
  providedIn: 'root',
})
export class UserContextService {
  private readonly userContextState = new BehaviorSubject<DataState<CurrentUser>>(
    createInitialState(),
  );

  private readonly userApiService = inject(UserApiService);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  readonly userContext$ = this.userContextState.asObservable();
  readonly user$ = this.userContext$.pipe(
    map(state => state.data),
    distinctUntilChanged(),
    shareReplay(1),
  );
  readonly loading$ = this.userContext$.pipe(
    map(state => state.loading),
    distinctUntilChanged(),
  );
  readonly error$ = this.userContext$.pipe(
    map(state => state.error),
    distinctUntilChanged(),
  );

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

  getCurrentUser(): CurrentUser | null {
    return this.userContextState.value.data;
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
          this.userContextState.next(createSuccessState(user));
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
    this.userContextState.next(createLoadingState(currentUser));
  }

  // Set error state
  private setError(error: string): void {
    const currentUser = this.getCurrentUser();
    this.userContextState.next(createErrorState(error, currentUser));
  }

  // Reset user context
  resetUserContext(): void {
    this.userContextState.next(createInitialState());
  }

  // Update user context
  updateUserContext(user: CurrentUser): void {
    this.userContextState.next(createSuccessState(user));
  }
}
