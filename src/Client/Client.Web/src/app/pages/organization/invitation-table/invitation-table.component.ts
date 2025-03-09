import { Component, inject } from '@angular/core';
import { TableWithPaginationComponent } from '@shared/components/table-with-pagination/table-with-pagination.component';
import { ColumnDefinition } from '@shared/components/table/table.component';
import { SearchInputComponent } from '@shared/components/search-input/search-input.component';
import { DatePipe } from '@angular/common';
import { TagComponent } from '@shared/components/tag/tag.component';
import { HeaderComponent } from '@shared/components/header/header.component';
import { OrganizationService } from '@features/services/organization.service';
import { Invitation, InvitationStatus } from '@features/models/invitation/invitation';
import { RouterLink } from '@angular/router';

function getStatusVariant(status: number): 'yellow' | 'green' | 'red' | 'gray' {
  switch (status) {
    case 0: return 'yellow';
    case 1: return 'green';
    case 2: return 'red';
    case 3: return 'gray';
    default: return 'gray';
  }
}

@Component({
  selector: 'app-invitation-table',
  imports: [TableWithPaginationComponent, SearchInputComponent, HeaderComponent, RouterLink],
  providers: [DatePipe],
  templateUrl: './invitation-table.component.html'
})
export class InvitationTableComponent {
  private organizationService = inject(OrganizationService);

  state$ = this.organizationService.invitationsState$;
  paginationOptions$ = this.organizationService.invitationsPaginationOptions$;

  onGetData() {
    this.organizationService.getInvitations().subscribe();
  }

  columnsConfig: ColumnDefinition<Invitation>[] = [
    {
      title: "Name",
      modelProp: 'userName'
    },
    {
      title: "Date of creation",
      modelProp: 'createdAt',
      pipe: DatePipe,
      pipeArgs: ['dd/MM/yyyy HH:mm:ss'],
      isSortable: true,
    },
    {
      title: "Status",
      modelProp: 'status',
      component: {
        type: TagComponent,
        inputs: (invitation: Invitation) => ({
          variant: getStatusVariant(invitation.status),
          text: InvitationStatus[invitation.status]
        })
      },
      isSortable: true,
    }
  ];
}
