import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from '@shared/services/base-api.service';
import { environment } from '@environments/environment';
import { AvailabilityTemplate } from '../models/availability-template.model';
import { AvailabilityTemplateResponse } from '../models/availability-template-response.model';
import { GetAvailabilityTemplatesResponse } from '../models/get-availability-templates-response.model';

@Injectable({
  providedIn: 'root',
})
export class AvailabilityTemplateApiService extends BaseApiService<AvailabilityTemplateResponse> {
  override readonly apiUrl = `${environment.apiUrl}/api/calendar/api/availabilitytemplates`;

  override create<AvailabilityTemplate>(
    payload: AvailabilityTemplate,
  ): Observable<AvailabilityTemplateResponse> {
    return this.http.post<AvailabilityTemplateResponse>(this.apiUrl, payload);
  }
  getAvailabilityTemplatesByUser(
    userId: string,
    page = 0,
    size = 10,
  ): Observable<GetAvailabilityTemplatesResponse> {
    return this.http.post<GetAvailabilityTemplatesResponse>(
      `${environment.apiUrl}/api/calendar/api/myavailabilitytemplates?page=${page}&size=${size}`,
      `"${userId}"`,
      {
        headers: {
          'Content-Type': 'application/json',
        },
      },
    );
  }
}
