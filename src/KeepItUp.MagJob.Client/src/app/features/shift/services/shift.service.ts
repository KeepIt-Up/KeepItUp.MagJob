import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { Shift } from '../models/shift.model';
import { CreateShiftPayload } from './shift.api';
import { StateService } from '../../../shared/services/state.service';
import { ShiftApiService } from './shift.api';
import { PaginatedResponse, PaginationOptions } from '@shared/components/pagination/pagination.component';
import { NotificationService } from '@shared/services/notification.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ShiftService {
    private stateService = new StateService<Shift>();
    
    private apiService = inject(ShiftApiService);
    private http = inject(HttpClient);
    private readonly apiUrl = `${environment.apiUrl}/api/workevidence/shifts`;
    state$ = this.stateService.state$;
    $shift = computed(() => this.stateService.state$().data);
    
    // paginationOptions$ = signal<PaginationOptions<Shift>>({
    //     pageNumber: 1,
    //     pageSize: 10,
    //     sortField: 'startTime',
    //     ascending: false,
    // });

    // getShift(shiftId: string) {
    //     return this.apiService.getShiftById(shiftId).pipe(
    //         tap(shift => {
    //             console.log('Shift loaded:', shift);
    //           this.stateService.setData(shift);
    //         }),
    //         catchError(error => {
    //           this.stateService.setError(error);
    //           console.error('Error loading shift:', error);
    //           return throwError(() => error);
    //         }),
    //       );
    // }
    
    // getShifts(query: Record<string, any> = {}) {
    //     return this.apiService.getShifts(query, this.paginationOptions$()).pipe(
    //         tap((response: PaginatedResponse<Shift>) => {
    //             this.stateService.setData(response.items[0]); // Store first shift in state
    //         }),
    //         catchError(error => {
    //             this.stateService.setError(error);
    //             return throwError(() => error);
    //         }),
    //     );
    // }
    
    // createShift(payload: CreateShiftPayload): Observable<any> {
    //     this.stateService.setLoading(true);
    //     return this.apiService.create(payload).pipe(
    //     tap(shift => {
    //         this.stateService.setData(shift);
    //     }),
    //     catchError(error => {
    //         this.stateService.setError(error);
    //         return throwError(() => error);
    //     }),
    //     );
    // }

    // Dodaj publiczną metodę do pobierania zmiany po id
    public getShiftById(id: string): Observable<Shift> {
        return this.apiService.getShiftById(id);
    }
}
