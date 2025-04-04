import { Component, inject, Input } from '@angular/core';
import { ColumnDefinition } from '@shared/components/table/table.component';
import { TableWithPaginationComponent } from '@shared/components/table-with-pagination/table-with-pagination.component';
import { SearchInputComponent } from '@shared/components/search-input/search-input.component';
import { RouterLink } from '@angular/router';
import { MemberActionsComponent } from './member-actions.component';
import { HeaderComponent } from '@shared/components/header/header.component';
import { EditMemberModalComponent } from '../../../features/members/components/edit-member-modal/edit-member-modal.component';
import { MemberService } from '../../../features/members/services/member.service';
import { Member } from '../../../features/members/models/member';

@Component({
  selector: 'app-members-table',
  imports: [
    HeaderComponent,
    TableWithPaginationComponent,
    SearchInputComponent,
    RouterLink,
    EditMemberModalComponent,
  ],
  templateUrl: './member-table.component.html',
})
export class MembersTableComponent {
  @Input() organizationId!: string;

  private memberService = inject(MemberService);
  state$ = this.memberService.membersState$;
  paginationOptions$ = this.memberService.membersPaginationOptions$;

  isEditModalOpen = false;
  selectedMember?: Member;

  onGetData() {
    this.memberService
      .getMembersByOrganizationId('9fce6f8f-7f24-4236-9418-9c4645b36fb9')
      .subscribe();
  }

  columnsConfig: ColumnDefinition<Member>[] = [
    {
      title: 'Name',
      modelProp: 'firstName',
      computeValue: row => {
        return row.firstName + ' ' + row.lastName;
      },
      isSortable: true,
    },
    {
      title: 'Is Still Member?',
      modelProp: 'archived',
      isSortable: true,
      computeValue: row => {
        return row.archived ? '' : '✅';
      },
    },
    {
      title: 'Role',
      modelProp: 'roles',
      isSortable: true,
      computeValue: row => {
        return row.roles.map(role => role.name).join(', ');
      },
    },
    {
      title: 'Actions',
      modelProp: 'id',
      component: {
        type: MemberActionsComponent,
        inputs: row => ({
          memberId: row.id,
          onEditCallback: () => this.openEditModal(row),
          onDeleteCallback: () => {
            this.memberService.archiveMember(row.id).subscribe({
              next: () => {
                this.paginationOptions$().pageNumber = 1;
              },
            });
          },
        }),
      },
    },
  ];

  openEditModal(member: Member) {
    this.selectedMember = member;
    this.isEditModalOpen = true;
  }

  closeEditModal() {
    this.isEditModalOpen = false;
    this.selectedMember = undefined;
  }

  saveMember(updatedMember: Partial<Member>) {
    if (this.selectedMember) {
      this.memberService.updateMember(this.selectedMember.id, updatedMember).subscribe({
        next: () => {
          this.closeEditModal();
          // Refresh the table
          this.paginationOptions$().pageNumber = 1;
        },
      });
    }
  }
}
