import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil, combineLatest, filter } from 'rxjs';
import {
  ChatListComponent,
  ChatMessagesComponent,
  ChatCreateModalComponent,
  ChatService,
  ChatMemberService,
  Chat,
} from './';
import { useOrganization } from '@organizations/hooks/use-organization';
import { UserContextService } from '@users/services/user-context.service';
import { ScrollControlService } from '@shared/services/scroll-control.service';

@Component({
  selector: 'app-chats',
  standalone: true,
  imports: [CommonModule, ChatListComponent, ChatMessagesComponent, ChatCreateModalComponent],
  templateUrl: './chats.component.html',
  styleUrls: ['./chats.component.scss'],
})
export class ChatsComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private chatService = inject(ChatService);
  private chatMemberService = inject(ChatMemberService);
  private destroy$ = new Subject<void>();
  private organizationContext = useOrganization();
  private userContextService = inject(UserContextService);
  private scrollControlService = inject(ScrollControlService);

  organizationId!: string;
  chats: Chat[] = [];
  selectedChat: Chat | null = null;
  showCreateModal = false;
  loading = false;

  get currentMemberId(): string | null {
    const currentUser = this.userContextService.getCurrentUser();
    const currentOrg = this.organizationContext.getCurrentOrganization();

    if (!currentUser || !currentOrg) return null;

    const membership = currentUser.memberships.find(m => m.organizationId === currentOrg.id);

    return membership?.memberId ?? null;
  }

  ngOnInit(): void {
    this.scrollControlService.setScrollable(false);

    combineLatest([this.userContextService.userContext$, this.organizationContext.organization$])
      .pipe(
        filter(([userState, orgData]) => userState.data !== null && orgData !== null),
        takeUntil(this.destroy$),
      )
      .subscribe(([userState, orgData]) => {
        if (userState.data && orgData) {
          this.organizationId = orgData.id;
          this.loadChats();
        }
      });

    this.chatService.chats$.pipe(takeUntil(this.destroy$)).subscribe(chats => {
      this.chats = chats;
    });

    this.chatService.selectedChat$.pipe(takeUntil(this.destroy$)).subscribe(chat => {
      this.selectedChat = chat;
    });
  }

  ngOnDestroy(): void {
    this.scrollControlService.setScrollable(true);

    this.destroy$.next();
    this.destroy$.complete();
  }

  loadChats(): void {
    this.loading = true;
    console.log('currentMemberId', this.currentMemberId);
    this.chatService
      .getChatsByMemberId(this.currentMemberId ?? '')
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: chats => {
          this.chatService.setChats(chats);
          this.loading = false;
        },
        error: error => {
          console.error('Error loading chats:', error);
          this.loading = false;
        },
      });
    console.log('chats', this.chats);
  }

  onChatSelect(chat: Chat): void {
    this.selectedChat = chat;

    this.chatService.setSelectedChat(chat);

    if (!chat.chatMembers || chat.chatMembers.length === 0) {
      this.loadChatMembers(chat);
    }
  }

  onCreateChat(): void {
    this.showCreateModal = true;
  }

  onChatCreated(chat: Chat): void {
    const currentChats = this.chatService.chatsSubject.value;
    this.chatService.setChats([chat, ...currentChats]);

    this.selectedChat = chat;

    this.chatService.setSelectedChat(chat);

    this.showCreateModal = false;
  }

  onModalClose(): void {
    this.showCreateModal = false;
  }

  private loadChatMembers(chat: Chat): void {
    this.chatMemberService.getChatMembersByChat(chat.id).subscribe({
      next: response => {
        const updatedChat = {
          ...chat,
          chatMembers: response.chatMembers.map(cm => ({
            id: cm.id,
            nickname: cm.nickname,
            memberId: cm.memberId,
            isInvitationAccepted: true,
            isAdmin: false,
          })),
        };

        const chatIndex = this.chats.findIndex(c => c.id === chat.id);
        if (chatIndex !== -1) {
          this.chats[chatIndex] = updatedChat;
        }

        if (this.selectedChat?.id === chat.id) {
          this.selectedChat = updatedChat;
        }
      },
      error: error => {
        console.error('Error loading chat members:', error);
      },
    });
  }

  onChatLeft(chatId: string): void {
    console.log('Chat left:', chatId);
  }
}
