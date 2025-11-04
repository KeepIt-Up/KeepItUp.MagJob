import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { environment } from '@environments/environment';
import { BaseApiService } from '@shared/services/base-api.service';
import { ShiftEditRequest } from '../../shift/models/shiftEditRequest.model';
import {
  PaginatedResponse,
  PaginationOptions,
  serializePaginationOptions,
} from '@shared/components/pagination/pagination.component';
import { catchError } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';

export interface CreateShiftEditPayload {
  newStartTime: string;
  newEndTime: string;
  status: string;
  shiftId: string;
  description: string;
}

export interface UpdateShiftEditRequestPayload {
  startTime?: string;
  endTime?: string;
  status?: string;
  description?: string;
}

@Injectable({
  providedIn: 'root',
})
export class ShiftEditRequestApiService extends BaseApiService<ShiftEditRequest> {
  override readonly apiUrl = `${environment.apiUrl}/api/workevidence/api/shift-edit-requests`;

  getEditRequests(
    shiftId: string,
    page: number = 0,
    size: number = 10
  ): Observable<PaginatedResponse<ShiftEditRequest>> {
    return this.http.get<PaginatedResponse<ShiftEditRequest>>(`${this.apiUrl}/shift/${shiftId}`, {
      params: { page: page.toString(), size: size.toString() }
    }).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Error fetching edit requests:', error);
        return this.handleError(error);
      })
    );
  }

  override create<TPayload = CreateShiftEditPayload>(payload: TPayload): Observable<ShiftEditRequest> {
    return this.http.post<ShiftEditRequest>(this.apiUrl, payload).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Error creating edit request:', error);
        return this.handleError(error);
      })
    );
  }

  override update<TPayload = UpdateShiftEditRequestPayload>(id: string, payload: TPayload): Observable<ShiftEditRequest> {
    return this.http.patch<ShiftEditRequest>(`${this.apiUrl}/${id}`, payload).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Error updating edit request:', error);
        return this.handleError(error);
      })
    );
  }

  override delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Error deleting edit request:', error);
        return this.handleError(error);
      })
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Wystąpił błąd podczas operacji.';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Błąd: ${error.error.message}`;
    } else {
      errorMessage = `Kod błędu: ${error.status}, wiadomość: ${error.message}`;
    }
    return throwError(() => new Error(errorMessage));
  }
}
