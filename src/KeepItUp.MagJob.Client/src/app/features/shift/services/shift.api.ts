import { environment } from '@environments/environment';
import { Shift } from '../models/shift.model';
import { inject, Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BaseApiService } from '@shared/services/base-api.service';
import { PaginatedResponse, PaginationOptions, serializePaginationOptions } from '@shared/components/pagination/pagination.component';
import { UserContextService } from '@users/services/user-context.service';
export interface CreateShiftPayload {
    startTime: string;
    description?: string;
    memberId: string;
}

@Injectable({
  providedIn: 'root',
}) 
export class ShiftApiService {
    private readonly apiUrl = `${environment.apiUrl}/api/workevidence`;
    private userContextService = inject(UserContextService);
    private readonly memberId = this.userContextService.getCurrentUser()?.id;
    private http = inject(HttpClient);

    getActiveShift(): Observable<Shift> {
        return this.http.get<Shift>(`${this.apiUrl}/shifts/active/${this.memberId}`).pipe(
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
            .put<void>(`${this.apiUrl}/shifts/end/${shiftId}`, {})
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
    getAllShiftsForMember(memberId: string): Observable<Shift[]> {
        return this.http
        .get<{ shifts: Shift[] }>(`${this.apiUrl}/shifts/all/${memberId}`)
        .pipe(
            map(response => response.shifts), // <-- wyciągamy tablicę shifts
            catchError((error: HttpErrorResponse) => {
                console.error('Error getting all shifts for member:', error);
                return throwError(() => new Error(error.message));
            })
        );
    }
}
  

