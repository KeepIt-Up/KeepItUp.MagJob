import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { Member } from '../../../models/member/member';

@Component({
    selector: 'app-edit-member-modal',
    imports: [CommonModule, FormsModule, ModalComponent],
    templateUrl: './edit-member-modal.component.html'
})
export class EditMemberModalComponent {
  @Input() isOpen = false;
  @Input() member?: Member;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<Partial<Member>>();

  memberData: Partial<Member> = {};

  ngOnChanges() {
    if (this.member) {
      this.memberData = { ...this.member };
    }
  }

  onSubmit() {
    this.save.emit(this.memberData);
  }
}
