import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { HeaderComponent } from '@shared/components/header/header.component';
import { InfiniteListComponent } from '@shared/components/infinite-list/infinite-list.component';
import { UserService } from '../../../features/users/services/user.service';
import { InvitationCardComponent } from '../../../features/invitations/components/invitation-card/invitation-card.component';
import { InvitationService } from '../../../features/invitations/services/invitation.service';

@Component({
  selector: 'app-user-invitations',
  imports: [HeaderComponent, CommonModule, InfiniteListComponent, InvitationCardComponent],
  templateUrl: './user-invitations.component.html',
})
export class UserInvitationsComponent {
  queryParams = computed(() => ({ userId: this.userState$().data?.id }));

  private userService = inject(UserService);
  userState$ = this.userService.userState$;

  invitationService = inject(InvitationService);
  invitationState$ = this.userService.invitationState$;
  paginationOptions$ = this.userService.invitationsPaginationOptions$;

  acceptInvitation(invitationId: string): void {
    this.invitationService.acceptInvitation(invitationId).subscribe();
  }

  rejectInvitation(invitationId: string): void {
    this.invitationService.rejectInvitation(invitationId).subscribe();
  }

  loadMore(): void {
    this.userService.getUserInvitations(this.queryParams()).subscribe();
  }
}
