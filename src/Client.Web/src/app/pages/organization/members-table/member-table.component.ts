import { Component, inject, Input, OnInit } from '@angular/core';
import { ColumnDefinition } from '@shared/components/table/table.component';
import { TableWithPaginationComponent } from '@shared/components/table-with-pagination/table-with-pagination.component';
import { SearchInputComponent } from '@shared/components/search-input/search-input.component';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { MemberActionsComponent } from './member-actions.component';
import { EditMemberModalComponent } from '@features/components/members/edit-member-modal/edit-member-modal.component';
import { Member } from '@features/models/member/member';
import { HeaderComponent } from '@shared/components/header/header.component';
import { OrganizationService } from '@features/services/organization.service';
import { MemberService } from '@features/services/member.service';

@Component({
  selector: 'app-members-table',
  imports: [HeaderComponent, TableWithPaginationComponent, SearchInputComponent, RouterLink, EditMemberModalComponent],
  templateUrl: './member-table.component.html'
})
export class MembersTableComponent {
  @Input() organizationId!: string;

  private memberService = inject(MemberService);
  state$ = this.memberService.membersState$;
  paginationOptions$ = this.memberService.membersPaginationOptions$;

  isEditModalOpen = false;
  selectedMember?: Member;

  onGetData() {
    this.memberService.getMembersByOrganizationId("0193eaf4-48a4-7723-9c4c-f3e6ec5cba4c").subscribe();
  }

  columnsConfig: ColumnDefinition<Member>[] = [
    {
      title: "Name",
      modelProp: 'firstName',
      computeValue: (row) => { return row.firstName + " " + row.lastName },
      isSortable: true
    },
    {
      title: "Is Still Member?",
      modelProp: 'archived',
      isSortable: true,
      computeValue: (row) => { return row.archived ? "" : "✅" }
    },
    {
      title: "Role",
      modelProp: 'roles',
      isSortable: true,
      computeValue: (row) => { return row.roles.map(role => role.name).join(", ") }
    },
    {
      title: "Actions",
      modelProp: 'id',
      component: {
        type: MemberActionsComponent,
        inputs: (row) => ({
          memberId: row.id,
          onEditCallback: () => this.openEditModal(row),
          onDeleteCallback: () => {
            this.memberService.archiveMember(row.id).subscribe({
              next: (res) => {
                this.paginationOptions$().pageNumber = 1;
              }
            });
          }
        }),
      }
    }
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
        }
      });
    }
  }
}
