import { Component, Input, output } from '@angular/core';
import { TagComponent } from '@shared/components/tag/tag.component';
import { CommonModule } from '@angular/common';
import { Invitation, InvitationStatus } from '@features/models/invitation/invitation';

@Component({
    selector: 'app-invitation-card',
    imports: [TagComponent, CommonModule],
    templateUrl: './invitation-card.component.html'
})
export class InvitationCardComponent {
  @Input({ required: true }) invitation!: Invitation;
  acceptInvitation = output<string>();
  rejectInvitation = output<string>();

  InvitationStatus = InvitationStatus;

  getStatusVariant(status: number): 'yellow' | 'green' | 'red' | 'gray' {
    switch (status) {
      case 0: return 'yellow';
      case 1: return 'green';
      case 2: return 'red';
      case 3: return 'gray';
      default: return 'gray';
    }
  }
}
