import { inject, Injectable } from '@angular/core';
import { InvitationApiService } from '@features/apis/invitation.api.service';
import { catchError, EMPTY, Observable, tap } from 'rxjs';
import { NotificationService } from '@shared/services/notification.service';

@Injectable({
  providedIn: 'root'
})
export class InvitationService {

  private apiService = inject(InvitationApiService);
  private notificationService = inject(NotificationService);


  acceptInvitation(invitationId: string): Observable<void> {
    return this.apiService.acceptInvitation(invitationId).pipe(
      tap(() => {
        this.notificationService.show('Invitation accepted successfully', 'success');
      }),
      catchError(() => {
        this.notificationService.show('Error accepting invitation', 'error');
        return EMPTY;
      })
    );
  }

  rejectInvitation(invitationId: string): Observable<void> {
    return this.apiService.rejectInvitation(invitationId).pipe(
      tap(() => {
        this.notificationService.show('Invitation rejected successfully', 'success');
      }),
      catchError(() => {
        this.notificationService.show('Error rejecting invitation', 'error');
        return EMPTY;
      })
    );
  }

  cancelInvitation(invitationId: string): Observable<void> {
    return this.apiService.cancelInvitation(invitationId).pipe(
      tap(() => {
        this.notificationService.show('Invitation cancelled successfully', 'success');
      }),
      catchError(() => {
        this.notificationService.show('Error cancelling invitation', 'error');
        return EMPTY;
      })
    );
  }
}
