import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { GraphicApiService } from './graphic.api.service';
import { UserContextService } from '../../../features/users/services/user-context.service';
import {
  GraphicResponse,
  GetGraphicsResponse,
  CreateTimeEntryMembersBulkRequest,
  TimeEntryMemberResponse,
} from '../models/graphic.model';

@Injectable({
  providedIn: 'root',
})
export class GraphicsService {
  private readonly graphicApiService = inject(GraphicApiService);
  private readonly userContextService = inject(UserContextService);

  loadGraphics(page = 0, size = 10): Observable<GetGraphicsResponse> {
    const currentUser = this.userContextService.getCurrentUser();
    if (!currentUser) {
      throw new Error('User not authenticated');
    }

    const userId = currentUser.id;
    // Now pass the userId to the API service
    return this.graphicApiService.getGraphicsByManager(userId, page, size);
  }

  getGraphic(id: string): Observable<GraphicResponse> {
    return this.graphicApiService.getGraphic(id);
  }

  addMemberToTimeEntry(timeEntryId: string, userId: string): Observable<void> {
    return this.graphicApiService.addMemberToTimeEntry(timeEntryId, userId);
  }

  removeMemberFromTimeEntry(timeEntryId: string, memberId: string): Observable<void> {
    return this.graphicApiService.removeMemberFromTimeEntry(timeEntryId, memberId);
  }

  createTimeEntryMembersBulk(
    request: CreateTimeEntryMembersBulkRequest,
  ): Observable<TimeEntryMemberResponse[]> {
    return this.graphicApiService.createTimeEntryMembersBulk(request);
  }
}
