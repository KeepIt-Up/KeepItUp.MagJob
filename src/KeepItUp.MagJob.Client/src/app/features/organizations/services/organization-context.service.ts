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
import { CurrentOrganization } from '../models/current-organization.model';
import { OrganizationApiService } from './organization.api.service';
import { errorState, initialState, loadedState, loadingState, State } from '@shared/state';

@Injectable({
  providedIn: 'root',
})
export class OrganizationContextService {
  private readonly organizationContextState =
    signal<State<CurrentOrganization>>(initialState<CurrentOrganization>());

  private readonly organizationApiService = inject(OrganizationApiService);

  readonly $organizationContext = this.organizationContextState.asReadonly();

  getCurrentOrganization(): CurrentOrganization | undefined {
    return this.organizationContextState().data;
  }

  loadOrganization(organizationId: string): Observable<CurrentOrganization | null> {
    if (!organizationId) {
      const error = 'Organization ID is not provided';
      this.setError(error);
      return of(null);
    }

    this.setLoading();

    return this.organizationApiService.get(organizationId).pipe(
      tap({
        next: organization => {
          this.organizationContextState.set(loadedState(organization));
        },
        error: (error: unknown) => {
          const errorMessage =
            error instanceof Error ? error.message : 'Failed to load organization data';
          this.setError(errorMessage);
        },
      }),
      catchError(error => {
        const errorMessage =
          error instanceof Error ? error.message : 'Failed to load organization data';
        this.setError(errorMessage);
        return of(null);
      }),
    );
  }

  // Set loading state
  private setLoading(): void {
    this.organizationContextState.set(loadingState(this.$organizationContext()));
  }

  // Set error state
  private setError(error: string): void {
    this.organizationContextState.set(errorState(error));
  }

  // Reset organization context
  resetOrganizationContext(): void {
    this.organizationContextState.set(initialState<CurrentOrganization>());
  }

  // Update organization context
  updateOrganizationContext(organization: CurrentOrganization): void {
    this.organizationContextState.set(loadedState(organization));
  }
}
