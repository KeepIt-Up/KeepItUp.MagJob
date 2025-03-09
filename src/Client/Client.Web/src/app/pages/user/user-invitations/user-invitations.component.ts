import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { HeaderComponent } from '@shared/components/header/header.component';
import { UserService } from '@features/services/user.service';
import { InvitationService } from '@features/services/invitation.service';
import { InfiniteListComponent } from '@shared/components/infinite-list/infinite-list.component';
import { InvitationCardComponent } from '@features/components/invitations/invitation-card/invitation-card.component';

@Component({
  selector: 'app-user-invitations',
  imports: [HeaderComponent, CommonModule, InfiniteListComponent, InvitationCardComponent],
  templateUrl: './user-invitations.component.html'
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
