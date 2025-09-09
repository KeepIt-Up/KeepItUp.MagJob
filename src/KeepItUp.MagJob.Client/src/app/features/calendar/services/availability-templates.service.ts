import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, catchError, tap, of } from 'rxjs';
import { AvailabilityTemplateApiService } from './availability-template.api.service';
import { AvailabilityTemplateResponse } from '../models/availability-template-response.model';
import { GetAvailabilityTemplatesResponse } from '../models/get-availability-templates-response.model';
import { UserContextService } from '../../users/services/user-context.service';
import {
  DataState,
  createInitialState,
  createLoadingState,
  createSuccessState,
  createErrorState,
} from '../../../shared/data-state/data-state.model';

@Injectable({
  providedIn: 'root',
})
export class AvailabilityTemplatesService {
  private readonly availabilityTemplateApiService = inject(AvailabilityTemplateApiService);
  private readonly userContextService = inject(UserContextService);
  private readonly templatesState = new BehaviorSubject<DataState<AvailabilityTemplateResponse[]>>(
    createInitialState<AvailabilityTemplateResponse[]>(),
  );

  private readonly paginationState = new BehaviorSubject<{
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalRecords: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
  }>({
    currentPage: 0,
    totalPages: 0,
    pageSize: 10,
    totalRecords: 0,
    hasNextPage: false,
    hasPreviousPage: false,
  });

  public readonly templatesState$ = this.templatesState.asObservable();
  public readonly paginationState$ = this.paginationState.asObservable();
  loadAvailabilityTemplates(
    page = 0,
    size = 10,
  ): Observable<GetAvailabilityTemplatesResponse | null> {
    const currentUser = this.userContextService.getCurrentUser();
    if (!currentUser) {
      const error = 'User not authenticated';
      this.templatesState.next(createErrorState<AvailabilityTemplateResponse[]>(error));
      return of(null);
    }

    this.templatesState.next(createLoadingState<AvailabilityTemplateResponse[]>());

    return this.availabilityTemplateApiService
      .getAvailabilityTemplatesByUser(currentUser.externalId, page, size)
      .pipe(
        tap({
          next: response => {
            this.templatesState.next(createSuccessState(response.availabilityTemplateResponseList));
            this.paginationState.next({
              currentPage: page,
              totalPages: 1,
              pageSize: size,
              totalRecords: response.availabilityTemplateResponseList.length,
              hasNextPage: response.availabilityTemplateResponseList.length === size,
              hasPreviousPage: page > 0,
            });
          },
          error: error => {
            const errorMessage =
              error instanceof Error ? error.message : 'Failed to load availability templates';
            this.templatesState.next(
              createErrorState<AvailabilityTemplateResponse[]>(errorMessage),
            );
          },
        }),
        catchError(error => {
          const errorMessage =
            error instanceof Error ? error.message : 'Failed to load availability templates';
          this.templatesState.next(createErrorState<AvailabilityTemplateResponse[]>(errorMessage));
          return of(null);
        }),
      );
  }
  loadMore(): void {
    const currentPagination = this.paginationState.value;
    if (currentPagination.hasNextPage) {
      this.loadAvailabilityTemplates(currentPagination.currentPage + 1, currentPagination.pageSize);
    }
  }

  reload(): void {
    this.loadAvailabilityTemplates(0, this.paginationState.value.pageSize);
  }
}
