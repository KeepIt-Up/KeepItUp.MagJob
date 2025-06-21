import { environment } from '@environments/environment';
import { Shift } from '../models/shift.model';
import { inject, Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BaseApiService } from '@shared/services/base-api.service';
import { PaginatedResponse, PaginationOptions, serializePaginationOptions } from '@shared/components/pagination/pagination.component';

export interface CreateShiftPayload {
    startTime: string;
    description?: string;
    memberId: number;
}

@Injectable({
  providedIn: 'root',
}) 
export class ShiftApiService {
    private readonly apiUrl = `${environment.apiUrl}/api/workevidence`;
    private http = inject(HttpClient);

    getActiveShift(): Observable<Shift> {
        return this.http.get<Shift>(`${this.apiUrl}/shifts/1`).pipe(
            catchError((error: HttpErrorResponse) => {
                console.error('Error getting active shift:', error);
                return throwError(() => new Error(error.message));
            })
        );
    }

    startShift(payload: CreateShiftPayload): Observable<Shift> {
        return this.http
            .post<Shift>(`${this.apiUrl}/shifts/start`, payload)
            .pipe(
                catchError((error: HttpErrorResponse) => {
                    console.error('Error starting shift:', {
                        status: error.status,
                        statusText: error.statusText,
                        error: error.error,
                        headers: error.headers,
                        url: error.url,
                        message: error.message
                    });
                    return throwError(() => new Error(error.message));
                })
            );
    }

    endShift(shiftId: string): Observable<void> {
        return this.http
            .post<void>(`${this.apiUrl}/shifts/${shiftId}/end`, {})
            .pipe(
                catchError((error: HttpErrorResponse) => {
                    console.error('Error ending shift:', {
                        status: error.status,
                        statusText: error.statusText,
                        error: error.error,
                        headers: error.headers,
                        url: error.url,
                        message: error.message
                    });
                    return throwError(() => new Error(error.message));
                })
            );
    }

    getShiftById(id: string): Observable<Shift> {
        return this.http
            .get<Shift>(`${this.apiUrl}/shifts/${id}`)
            .pipe(
                catchError((error: HttpErrorResponse) => {
                    console.error('Error details:', {
                        status: error.status,
                        statusText: error.statusText,
                        error: error.error,
                        headers: error.headers,
                        url: error.url,
                        message: error.message
                    });
                    return throwError(() => new Error(error.message));
                })
            );
    }
}
  

