import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { StateService } from '../../../shared/services/state.service';
import { NotificationService } from '../../../shared/services/notification.service';
import {
  PaginatedResponse,
  PaginationOptions,
} from '@shared/components/pagination/pagination.component';
import { ShiftEditRequest } from '../models/shiftEditRequest.model';
import { ShiftEditRequestApiService } from './shiftEditRequest.api.service';
import {
  CreateShiftEditPayload,
  UpdateShiftEditRequestPayload,
} from './shiftEditRequest.api.service';

@Injectable({
  providedIn: 'root',
})
export class ShiftEditRequestService {
  private stateService = new StateService<ShiftEditRequest>();
  private apiService = inject(ShiftEditRequestApiService);
  private shiftEditRequestStateService = new StateService<PaginatedResponse<ShiftEditRequest>>();
  private notificationService = inject(NotificationService);

  state$ = this.stateService.state$;
  $shiftEditRequest = computed(() => this.stateService.state$().data);
  shiftEditRequestsState$ = this.shiftEditRequestStateService.state$;

  shiftEditRequestsPaginationOptions$ = signal<PaginationOptions<ShiftEditRequest>>({
    pageNumber: 1,
    pageSize: 10,
    sortField: 'id',
    ascending: true,
  });

  getShiftEditRequest(shiftEditRequestId: string) {
    return this.apiService.get(shiftEditRequestId).pipe(
      tap(shiftEditRequest => {
        this.stateService.setData(shiftEditRequest);
      }),
      catchError(error => {
        this.stateService.setError(error);
        return throwError(() => error);
      }),
    );
  }

  updateShiftEditRequest(
    shiftEditRequestId: string,
    payload: UpdateShiftEditRequestPayload,
  ): Observable<ShiftEditRequest> {
    this.stateService.setLoading(true);
    return this.apiService.update(shiftEditRequestId, payload).pipe(
      tap(shiftEditRequest => {
        this.stateService.setData(shiftEditRequest);
        this.notificationService.show('Zmiana została zaktualizowana', 'success');
      }),
      catchError(error => {
        this.stateService.setError(error);
        this.notificationService.show('Nie udało się zaktualizować zmiany', 'error');
        return throwError(() => error);
      }),
    );
  }

  createShiftEditRequest(payload: CreateShiftEditPayload): Observable<ShiftEditRequest> {
    this.stateService.setLoading(true);
    return this.apiService.create(payload).pipe(
      tap(shiftEditRequest => {
        this.stateService.setData(shiftEditRequest);
        this.notificationService.show('Wniosek o zmianę został utworzony', 'success');
      }),
      catchError(error => {
        this.stateService.setError(error);
        this.notificationService.show('Nie udało się utworzyć wniosku o zmianę', 'error');
        return throwError(() => error);
      }),
    );
  }

  getShiftEditRequests(shiftId: string, page: number = 0, size: number = 10): Observable<PaginatedResponse<ShiftEditRequest>> {
    return this.apiService.getEditRequests(shiftId, page, size).pipe(
      tap((response: PaginatedResponse<ShiftEditRequest>) => {
        this.shiftEditRequestStateService.setData(response);
      }),
      catchError(error => {
        this.shiftEditRequestStateService.setError(error);
        return throwError(() => error);
      }),
    );
  }

  deleteShiftEditRequest(id: string): Observable<void> {
    return this.apiService.delete(id).pipe(
      tap(() => {
        this.notificationService.show('Wniosek o zmianę został usunięty', 'success');
      }),
      catchError(error => {
        this.notificationService.show('Nie udało się usunąć wniosku o zmianę', 'error');
        return throwError(() => error);
      }),
    );
  }
}
