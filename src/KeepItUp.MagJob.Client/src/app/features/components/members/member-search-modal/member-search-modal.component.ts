import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SearchModalComponent } from '@shared/components/search-modal/search-modal.component';
import { Member } from '../../../models/member/member';
import { MemberService } from '@features/services/member.service';

@Component({
  selector: 'app-member-search-modal',
  imports: [CommonModule, SearchModalComponent],
  template: `
    <app-search-modal
      [isOpen]="isOpen"
      [title]="'Search Members'"
      [searchPlaceholder]="'Search members...'"
      [paginationOptions$]="paginationOptions$()"
      [state$]="state$()"
      [selectedItems]="selectedMembers"
      [displayFn]="memberDisplayFn"
      [compareFn]="memberCompareFn"
      (close)="handleClose()"
      (search)="handleSearch($event)"
      (selectionChange)="memberToggled.emit($event)"
      (onLoad)="loadMore.emit()"
    >
    </app-search-modal>
  `,
})
export class MemberSearchModalComponent {
  @Input() isOpen = false;
  @Input() filteredMembers: Member[] = [];
  @Input() selectedMembers: Member[] = [];

  @Output() closeModal = new EventEmitter<void>();
  @Output() searchMembers = new EventEmitter<string>();
  @Output() memberToggled = new EventEmitter<Member>();
  @Output() loadMore = new EventEmitter<void>();
  private memberService = inject(MemberService);

  paginationOptions$ = this.memberService.memberSearchPaginationOptions$;
  state$ = this.memberService.memberSearchState$;

  memberDisplayFn(member: Member): string {
    return `${member.firstName} ${member.lastName}`;
  }

  memberCompareFn(a: Member, b: Member): boolean {
    return a.id === b.id;
  }

  handleClose(): void {
    this.closeModal.emit();
  }

  handleSearch(query: string): void {
    this.searchMembers.emit(query);
  }
}
