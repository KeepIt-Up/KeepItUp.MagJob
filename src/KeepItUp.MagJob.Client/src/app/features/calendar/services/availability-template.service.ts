import { computed, inject, Injectable } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { NotificationService } from '@shared/services/notification.service';
import { StateService } from '@shared/services/state.service';
import { AvailabilityTemplateApiService } from './availability-template.api.service';
import { AvailabilityTemplate } from '../models/availability-template.model';
import { AvailabilityTemplateResponse } from '../models/availability-template-response.model';

@Injectable({
  providedIn: 'root',
})
export class AvailabilityTemplateService {
  private stateService = new StateService<AvailabilityTemplateResponse>();

  private apiService = inject(AvailabilityTemplateApiService);
  private notificationService = inject(NotificationService);

  state$ = this.stateService.state$;
  $availabilityTemplate = computed(() => this.stateService.state$().data);

  createAvailabilityTemplate(
    payload: AvailabilityTemplate,
  ): Observable<AvailabilityTemplateResponse> {
    return this.apiService.create(payload).pipe(
      tap(template => {
        this.stateService.setData(template);
        this.notificationService.show('Availability template created successfully', 'success');
      }),
      catchError(error => {
        this.stateService.setError(error);
        this.notificationService.show('Failed to create availability template', 'error');
        return throwError(() => error);
      }),
    );
  }
}
