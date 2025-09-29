import { inject, Injectable } from '@angular/core';
import { InvitationApiService } from './invitation.api.service';
import { catchError, EMPTY, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class InvitationService {
  private apiService = inject(InvitationApiService);

  acceptInvitation(invitationId: string): Observable<void> {
    return this.apiService.acceptInvitation(invitationId).pipe(
      catchError(() => {
        return EMPTY;
      }),
    );
  }

  rejectInvitation(invitationId: string): Observable<void> {
    return this.apiService.rejectInvitation(invitationId).pipe(
      catchError(() => {
        return EMPTY;
      }),
    );
  }

  cancelInvitation(invitationId: string): Observable<void> {
    return this.apiService.cancelInvitation(invitationId).pipe(
      catchError(() => {
        return EMPTY;
      }),
    );
  }
}
