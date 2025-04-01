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
} from 'rxjs';
import { CurrentOrganization } from '../models/current-organization.model';
import {
  DataState,
  createInitialState,
  createLoadingState,
  createSuccessState,
  createErrorState,
} from '../../../shared/data-state/data-state.model';
import { OrganizationApiService } from './organization.api.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class OrganizationContextService {
  private readonly organizationContextState = new BehaviorSubject<DataState<CurrentOrganization>>(
    createInitialState(),
  );

  private readonly organizationApiService = inject(OrganizationApiService);
  private readonly router = inject(Router);

  readonly organizationContext$ = this.organizationContextState.asObservable();
  readonly organization$ = this.organizationContext$.pipe(
    map(state => state.data),
    distinctUntilChanged(),
    shareReplay(1),
  );
  readonly loading$ = this.organizationContext$.pipe(
    map(state => state.loading),
    distinctUntilChanged(),
  );
  readonly error$ = this.organizationContext$.pipe(
    map(state => state.error),
    distinctUntilChanged(),
  );

  getCurrentOrganization(): CurrentOrganization | null {
    return this.organizationContextState.value.data;
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
          this.organizationContextState.next(createSuccessState(organization));
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
    const currentOrganization = this.getCurrentOrganization();
    this.organizationContextState.next(createLoadingState(currentOrganization));
  }

  // Set error state
  private setError(error: string): void {
    const currentOrganization = this.getCurrentOrganization();
    this.organizationContextState.next(createErrorState(error, currentOrganization));
  }

  // Reset organization context
  resetOrganizationContext(): void {
    this.organizationContextState.next(createInitialState());
  }

  // Update organization context
  updateOrganizationContext(organization: CurrentOrganization): void {
    this.organizationContextState.next(createSuccessState(organization));
  }
}
