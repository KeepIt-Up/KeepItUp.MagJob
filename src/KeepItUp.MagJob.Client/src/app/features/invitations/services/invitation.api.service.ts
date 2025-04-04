import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Invitation } from '../models/invitation';
import { BaseApiService } from '@shared/services/base-api.service';
import { environment } from '@environments/environment';
@Injectable({
  providedIn: 'root',
})
export class InvitationApiService extends BaseApiService<Invitation> {
  override readonly apiUrl = `${environment.apiUrl}/api/identity/invitations`;

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
