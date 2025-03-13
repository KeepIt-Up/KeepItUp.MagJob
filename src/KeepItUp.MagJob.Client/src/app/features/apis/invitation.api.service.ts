import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Invitation } from '@features/models/invitation/invitation';
import { BaseApiService } from '@shared/services/base-api.service';

@Injectable({
  providedIn: 'root'
})
export class InvitationApiService extends BaseApiService<Invitation> {

  override readonly apiUrl = '/api/invitations';

  acceptInvitation(invitationId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${invitationId}/accept`, {});
  }

  rejectInvitation(invitationId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${invitationId}/reject`, {});
  }

  cancelInvitation(invitationId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${invitationId}/cancel`, {});
  }
}
