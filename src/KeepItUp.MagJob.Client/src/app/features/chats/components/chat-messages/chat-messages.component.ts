import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  OnDestroy,
  inject,
  HostListener,
  ViewChild,
  ElementRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import {
  Chat,
  ChatMessage,
  SendMessageRequest,
  ChatMember,
  TypingUser,
} from '../../models/chat.model';
import { ChatService } from '../../services/chat.service';
import { ChatMemberService } from '../../services/chat-member.service';
import { ChatAddMembersModalComponent } from '../chat-add-members-modal/chat-add-members-modal.component';

@Component({
  selector: 'app-chat-messages',
  standalone: true,
  imports: [CommonModule, FormsModule, ChatAddMembersModalComponent],
  templateUrl: './chat-messages.component.html',
  styleUrls: ['./chat-messages.component.scss'],
})
export class ChatMessagesComponent implements OnInit, OnDestroy {
  @Input() chat!: Chat;
  @Input() organizationId!: string;
  @Input() currentMemberId!: string | null;
  @Output() chatLeft = new EventEmitter<string>();

  private chatService = inject(ChatService);
  private chatMemberService = inject(ChatMemberService);
  private destroy$ = new Subject<void>();

  @ViewChild('dropdownMenu') dropdownMenuRef!: ElementRef<HTMLElement>;
  @ViewChild(ChatAddMembersModalComponent) addMembersModal!: ChatAddMembersModalComponent;

  messages: ChatMessage[] = [];
  newMessage = '';
  loading = false;
  showDropdown = false;
  showAddMembersModal = false;
  typingUsers: TypingUser[] = [];
  private typingTimeout: ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    console.log('ChatMessagesComponent ngOnInit - currentMemberId:', this.currentMemberId);
    this.loadMessages();
    this.subscribeToMessages();
    this.subscribeToTypingEvents();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadMessages(): void {
    this.loading = true;
    this.chatService.getChatMessages(this.chat.id).subscribe({
      next: response => {
        this.messages = response.chatMessages;
        this.loading = false;
        this.scrollToBottom();
      },
      error: error => {
        console.error('Error loading messages:', error);
        this.loading = false;
      },
    });
  }

  subscribeToMessages(): void {
    this.chatService.messages$.pipe(takeUntil(this.destroy$)).subscribe(messages => {
      const wasAtBottom = this.isScrolledToBottom();
      this.messages = messages;
      if (wasAtBottom) {
        this.scrollToBottomSmooth();
      }
    });
  }

  sendMessage(): void {
    if (!this.newMessage.trim()) return;

    console.log('Sending message:', {
      chat: this.chat,
      chatMembers: this.chat.chatMembers,
      currentMemberId: this.currentMemberId,
    });

    const currentChatMember = this.chat.chatMembers?.find(
      member => member.memberId === this.currentMemberId,
    );

    if (!currentChatMember) {
      console.error('Current user is not a member of this chat', {
        chatMembers: this.chat.chatMembers,
        currentMemberId: this.currentMemberId,
      });
      return;
    }

    const request: SendMessageRequest = {
      content: this.newMessage.trim(),
      chatId: this.chat.id,
      chatMemberId: currentChatMember.id,
    };

    console.log('Sending request:', request);
    this.chatService.sendMessage(request);
    this.newMessage = '';
    this.scrollToBottomSmooth();
  }

  onKeyPress(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    } else if (event.key === 'Enter' && event.shiftKey) {
      return;
    }
  }

  onInputChange(): void {
    this.handleTyping();
  }

  private handleTyping(): void {
    const currentChatMember = this.chat.chatMembers?.find(
      member => member.memberId === this.currentMemberId,
    );

    if (!currentChatMember) {
      return;
    }

    // Wyczyść poprzedni timeout
    if (this.typingTimeout) {
      clearTimeout(this.typingTimeout);
    }

    // Wyślij TYPING_START tylko jeśli nie jesteś już na liście piszących
    if (this.currentMemberId && !this.isCurrentlyTyping()) {
      this.chatService.sendTypingStart(
        this.chat.id,
        this.currentMemberId,
        currentChatMember.nickname ?? 'Unknown User',
      );
    }

    // Ustaw timeout na TYPING_STOP po 2 sekundach bez pisania
    this.typingTimeout = setTimeout(() => {
      if (this.currentMemberId) {
        this.chatService.sendTypingStop(
          this.chat.id,
          this.currentMemberId,
          currentChatMember.nickname ?? 'Unknown User',
        );
      }
    }, 2000);
  }

  private isCurrentlyTyping(): boolean {
    return this.typingUsers.some(user => user.memberId === this.currentMemberId);
  }

  private subscribeToTypingEvents(): void {
    this.chatService.typingUsers$.pipe(takeUntil(this.destroy$)).subscribe(typingUsers => {
      // Filtruj siebie z listy piszących
      this.typingUsers = typingUsers.filter(user => user.memberId !== this.currentMemberId);
    });

    // Subskrybuj do typing events dla tego chatu
    this.chatService.subscribeToTypingEvents(this.chat.id);
  }

  onTextareaInput(event: Event): void {
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.height = Math.min(textarea.scrollHeight, 128) + 'px';
  }

  isOwnMessage(message: ChatMessage): boolean {
    if (!this.currentMemberId || !message.chatMember?.memberId) {
      // console.log('isOwnMessage - missing IDs:', {
      //   currentMemberId: this.currentMemberId,
      //   messageChatMemberId: message.chatMember?.memberId,
      // });
      return false;
    }

    const isOwn = message.chatMember.memberId === this.currentMemberId;
    // console.log('isOwnMessage debug:', {
    //   messageId: message.id,
    //   messageChatMemberId: message.chatMember.memberId,
    //   currentMemberId: this.currentMemberId,
    //   isOwn: isOwn,
    //   messageContent: message.content.substring(0, 20) + '...',
    // });
    return isOwn;
  }

  formatMessageTime(date: Date | string): string {
    let parsed: Date;

    if (typeof date === 'string') {
      parsed = new Date(date + 'Z');
    } else {
      parsed = new Date(date);
    }

    if (isNaN(parsed.getTime())) {
      return '';
    }

    const dd = String(parsed.getDate()).padStart(2, '0');
    const mm = String(parsed.getMonth() + 1).padStart(2, '0');
    const yyyy = parsed.getFullYear();
    const hh = String(parsed.getHours()).padStart(2, '0');
    const min = String(parsed.getMinutes()).padStart(2, '0');

    return `${dd}.${mm}.${yyyy} ${hh}:${min}`;
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      const messageContainer = document.querySelector('.messages-container');
      if (messageContainer) {
        messageContainer.scrollTop = messageContainer.scrollHeight;
      }
    }, 50);
  }

  private scrollToBottomSmooth(): void {
    setTimeout(() => {
      const messageContainer = document.querySelector('.messages-container');
      if (messageContainer) {
        messageContainer.scrollTo({
          top: messageContainer.scrollHeight,
          behavior: 'smooth',
        });
      }
    }, 50);
  }

  private isScrolledToBottom(): boolean {
    const messageContainer = document.querySelector('.messages-container');
    if (!messageContainer) return true;

    const threshold = 100;
    return (
      messageContainer.scrollTop + messageContainer.clientHeight >=
      messageContainer.scrollHeight - threshold
    );
  }

  toggleDropdown(): void {
    this.showDropdown = !this.showDropdown;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    const target = event.target as HTMLElement;
    const dropdownElement = this.dropdownMenuRef?.nativeElement;

    if (this.showDropdown && dropdownElement && !dropdownElement.contains(target)) {
      this.showDropdown = false;
    }
  }

  leaveChat(): void {
    if (!this.currentMemberId) {
      console.error('Cannot leave chat: currentMemberId is null');
      return;
    }

    const currentChatMember = this.chat.chatMembers?.find(
      member => member.memberId === this.currentMemberId,
    );

    if (!currentChatMember) {
      console.error('Cannot leave chat: current user is not a member of this chat');
      return;
    }

    if (confirm('Czy na pewno chcesz opuścić ten czat?')) {
      this.chatMemberService.deleteChatMember(currentChatMember.id).subscribe({
        next: () => {
          console.log('Successfully left chat');
          this.showDropdown = false;

          this.chatService.removeChat(this.chat.id);

          this.chatService.leaveChat(this.chat.id);

          this.chatLeft.emit(this.chat.id);
        },
        error: error => {
          console.error('Error leaving chat:', error);
          alert('Wystąpił błąd podczas opuszczania czatu');
        },
      });
    }
  }

  openAddMembersModal(): void {
    this.showAddMembersModal = true;
    this.showDropdown = false;

    setTimeout(() => {
      if (this.addMembersModal) {
        console.log('Modal opened, triggering member load...');
        this.addMembersModal.loadMembersManually();
      }
    }, 100);
  }

  closeAddMembersModal(): void {
    this.showAddMembersModal = false;
  }

  onMembersAdded(addedMembers: ChatMember[]): void {
    if (!this.chat.chatMembers) {
      this.chat.chatMembers = [];
    }
    this.chat.chatMembers.push(...addedMembers);

    this.showAddMembersModal = false;

    console.log('Members added to chat:', addedMembers);
  }
}
