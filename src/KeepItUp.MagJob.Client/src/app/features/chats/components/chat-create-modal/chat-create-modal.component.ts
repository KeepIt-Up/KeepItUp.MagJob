import { Component, Input, Output, EventEmitter, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Chat, CreateChatRequest } from '../../models/chat.model';
import { ChatService } from '../../services/chat.service';
import { MemberService } from '@members/services/member.service';
import { PaginatedResponse } from '@shared/components/pagination/pagination.component';
import { Member } from '@members/models/member';
import { UserContextService } from '@users/services/user-context.service';
import { ChatMemberService } from '../../services/chat-member.service';

@Component({
  selector: 'app-chat-create-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './chat-create-modal.component.html',
  styleUrls: ['./chat-create-modal.component.scss'],
})
export class ChatCreateModalComponent implements OnInit {
  @Input() isOpen = false;
  @Input() organizationId = '';
  @Input() currentMemberId = '';
  @Output() chatCreated = new EventEmitter<Chat>();
  @Output() closeModal = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private chatService = inject(ChatService);
  private memberService = inject(MemberService);
  private chatMemberService = inject(ChatMemberService);
  private userContextService = inject(UserContextService);

  chatForm!: FormGroup;
  members: Member[] = [];
  selectedMembers: Member[] = [];
  loading = false;
  submitting = false;

  ngOnInit(): void {
    this.initForm();
    this.loadMembers();
  }

  private initForm(): void {
    this.chatForm = this.fb.group({
      title: this.fb.control<string>('', {
        validators: [Validators.required, Validators.minLength(3), Validators.maxLength(100)],
        nonNullable: true,
      }),
      memberIds: this.fb.control<string[]>([], {
        validators: [Validators.required, Validators.minLength(1)],
        nonNullable: true,
      }),
    });
  }

  private loadMembers(): void {
    this.loading = true;
    this.memberService.getMembersByOrganizationId(this.organizationId).subscribe({
      next: (members: PaginatedResponse<Member>) => {
        this.members = members.items.filter(m => m.id !== this.currentMemberId);
        this.loading = false;
      },
      error: error => {
        console.error('Error loading members:', error);
        this.loading = false;
      },
    });
  }

  onMemberToggle(member: Member): void {
    const index = this.selectedMembers.findIndex(m => m.id === member.id);
    if (index > -1) {
      this.selectedMembers.splice(index, 1);
    } else {
      this.selectedMembers.push(member);
    }

    const memberIds = this.selectedMembers.map(m => m.id);
    this.chatForm.patchValue({ memberIds });
  }

  isMemberSelected(member: Member): boolean {
    return this.selectedMembers.some(m => m.id === member.id);
  }

  onSubmit(): void {
    if (this.chatForm.valid && !this.submitting) {
      this.submitting = true;

      const currentUser = this.userContextService.getCurrentUser();
      if (!currentUser) {
        console.error('Current user not found');
        this.submitting = false;
        return;
      }

      const request: CreateChatRequest = {
        title: this.chatForm.value.title,
        organizationId: this.organizationId,
        memberId: this.currentMemberId,
        nickname: `${currentUser.firstName} ${currentUser.lastName}`,
      };

      this.chatService.createChat(request).subscribe({
        next: chat => {
          this.addSelectedMembersToChat(chat);
        },
        error: error => {
          console.error('Error creating chat:', error);
          this.submitting = false;
        },
      });
    }
  }

  private addSelectedMembersToChat(chat: Chat): void {
    if (this.selectedMembers.length === 0) {
      this.loadChatMembersAndEmit(chat);
      return;
    }

    let completedCount = 0;
    const totalMembers = this.selectedMembers.length;

    this.selectedMembers.forEach(member => {
      const chatMemberRequest = {
        nickname: `${member.firstName} ${member.lastName}`,
        memberId: member.id,
        chatId: chat.id,
      };

      this.chatMemberService.createChatMember(chatMemberRequest).subscribe({
        next: () => {
          completedCount++;
          if (completedCount === totalMembers) {
            this.loadChatMembersAndEmit(chat);
          }
        },
        error: error => {
          console.error(`Error adding member ${member.id} to chat:`, error);
          completedCount++;
          if (completedCount === totalMembers) {
            this.loadChatMembersAndEmit(chat);
          }
        },
      });
    });
  }

  private loadChatMembersAndEmit(chat: Chat): void {
    this.chatMemberService.getChatMembersByChat(chat.id).subscribe({
      next: chatMembersResponse => {
        const chatWithMembers = {
          ...chat,
          chatMembers: chatMembersResponse.chatMembers.map(cm => ({
            id: cm.id,
            nickname: cm.nickname,
            memberId: cm.memberId,
            isAdmin: false,
          })),
        };

        this.chatCreated.emit(chatWithMembers);
        this.resetForm();
        this.submitting = false;
      },
      error: error => {
        console.error('Error loading chat members:', error);
        this.chatCreated.emit(chat);
        this.resetForm();
        this.submitting = false;
      },
    });
  }

  onClose(): void {
    this.closeModal.emit();
    this.resetForm();
  }

  private resetForm(): void {
    this.chatForm.reset();
    this.selectedMembers = [];
  }

  get titleError(): string {
    const control = this.chatForm.get('title');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Chat title is required';
      if (control.errors['minlength']) return 'Title must be at least 3 characters';
      if (control.errors['maxlength']) return 'Title must be less than 100 characters';
    }
    return '';
  }

  get memberIdsError(): string {
    const control = this.chatForm.get('memberIds');
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Please select at least one member';
      if (control.errors['minlength']) return 'Please select at least one member';
    }
    return '';
  }
}
