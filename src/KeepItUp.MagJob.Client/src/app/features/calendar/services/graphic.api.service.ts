import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  PostCreateAndPopulateGraphic,
  CreateGraphicResponse,
} from '../models/post-create-and-populate-graphic.model';
import {
  GraphicResponse,
  GetGraphicsResponse,
  CreateTimeEntryMembersBulkRequest,
  TimeEntryMemberResponse,
} from '../models/graphic.model';

@Injectable({
  providedIn: 'root',
})
export class GraphicApiService {
  private readonly httpClient = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/calendar/api`;

  createAndPopulateGraphic(
    request: PostCreateAndPopulateGraphic,
  ): Observable<CreateGraphicResponse> {
    return this.httpClient.post<CreateGraphicResponse>(`${this.apiUrl}/createGraphic`, request);
  }

  getGraphicsByManager(userId: string, page = 0, size = 10): Observable<GetGraphicsResponse> {
    const url = `${this.apiUrl}/mygraphics`;
    console.log('Calling graphics endpoint:', url);
    console.log('Sending userId:', userId);
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    return this.httpClient.post<GetGraphicsResponse>(
      `${url}?page=${page}&size=${size}`,
      `"${userId}"`,
      { headers },
    );
  }

  getGraphic(id: string): Observable<GraphicResponse> {
    return this.httpClient.get<GraphicResponse>(`${this.apiUrl}/graphics/${id}`);
  }

  addMemberToTimeEntry(timeEntryId: string, userId: string): Observable<void> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    return this.httpClient.post<void>(
      `${this.apiUrl}/timeentries/${timeEntryId}/members`,
      `"${userId}"`, // Send as quoted JSON string
      { headers },
    );
  }

  removeMemberFromTimeEntry(timeEntryId: string, memberId: string): Observable<void> {
    return this.httpClient.delete<void>(
      `${this.apiUrl}/timeentries/${timeEntryId}/members/${memberId}`,
    );
  }

  createTimeEntryMembersBulk(
    request: CreateTimeEntryMembersBulkRequest,
  ): Observable<TimeEntryMemberResponse[]> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    return this.httpClient.post<TimeEntryMemberResponse[]>(
      `${this.apiUrl}/timeentrymembers/bulk`,
      request,
      { headers },
    );
  }
}
