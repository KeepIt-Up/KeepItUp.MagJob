import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { HttpClient } from '@angular/common/http';
@Injectable({
  providedIn: 'root',
})
export class InvitationApiService {
  readonly apiUrl = `${environment.apiUrl}/api/identity/invitations`;
  private readonly http = inject(HttpClient);

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
