import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  OnChanges,
  SimpleChanges,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Chat, ChatMember } from '../../models/chat.model';
import { ChatMemberService } from '../../services/chat-member.service';
import { MemberService } from '@members/services/member.service';
import { PaginatedResponse } from '@shared/components/pagination/pagination.component';
import { Member } from '@members/models/member';

@Component({
  selector: 'app-chat-add-members-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './chat-add-members-modal.component.html',
  styleUrls: ['./chat-add-members-modal.component.scss'],
})
export class ChatAddMembersModalComponent implements OnInit, OnChanges {
  @Input() isOpen = false;
  @Input() chat!: Chat;
  @Input() organizationId = '';
  @Output() membersAdded = new EventEmitter<ChatMember[]>();
  @Output() closeModal = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private memberService = inject(MemberService);
  private chatMemberService = inject(ChatMemberService);

  addMembersForm: FormGroup;
  members: Member[] = [];
  selectedMembers: Member[] = [];
  loading = false;
  submitting = false;

  constructor() {
    this.addMembersForm = this.fb.group({
      memberIds: this.fb.control<string[]>([], {
        validators: [Validators.required, Validators.minLength(1)],
        nonNullable: true,
      }),
    });
  }

  ngOnInit = (): void => {
    //
  };

  ngOnChanges = (changes: SimpleChanges): void => {
    if (changes['isOpen']?.currentValue === true) {
      this.loadMembers();
    }
    if (changes['chat']?.currentValue) {
      this.loadMembers();
    }
    if (changes['organizationId']?.currentValue) {
      this.loadMembers();
    }
  };

  public loadMembersManually = (): void => {
    this.loadMembers();
  };

  private loadMembers = (): void => {
    if (!this.organizationId || !this.chat) {
      return;
    }

    this.loading = true;

    this.memberService.getMembersByOrganizationId(this.organizationId).subscribe({
      next: (members: PaginatedResponse<Member>) => {
        const currentChatMemberIds = this.chat.chatMembers?.map(cm => cm.memberId) ?? [];
        this.members = members.items.filter(m => !currentChatMemberIds.includes(m.id));
        this.loading = false;
      },
      error: error => {
        console.error('Error loading members:', error);
        this.loading = false;
      },
    });
  };

  onMemberToggle = (member: Member): void => {
    const index = this.selectedMembers.findIndex(m => m.id === member.id);
    if (index > -1) {
      this.selectedMembers.splice(index, 1);
    } else {
      this.selectedMembers.push(member);
    }

    const memberIds = this.selectedMembers.map(m => m.id);
    if (this.addMembersForm) {
      this.addMembersForm.patchValue({ memberIds });
    }
  };

  isMemberSelected = (member: Member): boolean => {
    return this.selectedMembers.some(m => m.id === member.id);
  };

  onSubmit = (): void => {
    if (this.addMembersForm && this.addMembersForm.valid && !this.submitting) {
      this.submitting = true;

      let completedCount = 0;
      const totalMembers = this.selectedMembers.length;
      const addedMembers: ChatMember[] = [];

      this.selectedMembers.forEach(member => {
        const chatMemberRequest = {
          nickname: `${member.firstName} ${member.lastName}`,
          memberId: member.id,
          chatId: this.chat.id,
        };

        this.chatMemberService.createChatMember(chatMemberRequest).subscribe({
          next: chatMemberResponse => {
            const chatMember: ChatMember = {
              id: chatMemberResponse.id,
              nickname: chatMemberResponse.nickname,
              memberId: chatMemberResponse.memberId,
              isAdmin: false,
            };
            addedMembers.push(chatMember);
            completedCount++;
            if (completedCount === totalMembers) {
              this.membersAdded.emit(addedMembers);
              this.resetForm();
              this.submitting = false;
            }
          },
          error: error => {
            console.error(`Error adding member ${member.id} to chat:`, error);
            completedCount++;
            if (completedCount === totalMembers) {
              this.membersAdded.emit(addedMembers);
              this.resetForm();
              this.submitting = false;
            }
          },
        });
      });
    }
  };

  onClose = (): void => {
    this.closeModal.emit();
    this.resetForm();
  };

  private resetForm = (): void => {
    if (this.addMembersForm) {
      this.addMembersForm.reset();
    }
    this.selectedMembers = [];
  };

  get memberIdsError(): string {
    if (!this.addMembersForm) return '';
    const control = this.addMembersForm.get('memberIds');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Proszę wybrać przynajmniej jednego członka';
      if (control.errors['minlength']) return 'Proszę wybrać przynajmniej jednego członka';
    }
    return '';
  }
}
