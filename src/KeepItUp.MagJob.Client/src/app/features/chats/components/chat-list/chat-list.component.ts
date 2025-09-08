import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chat } from '../../models/chat.model';

@Component({
  selector: 'app-chat-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './chat-list.component.html',
  styleUrls: ['./chat-list.component.scss'],
})
export class ChatListComponent {
  @Input() chats: Chat[] = [];
  @Input() selectedChat: Chat | null = null;
  @Input() loading = false;
  @Output() chatSelect = new EventEmitter<Chat>();

  onChatSelect(chat: Chat): void {
    this.chatSelect.emit(chat);
  }

  getLastMessagePreview(chat: Chat): string {
    if (chat.lastMessage) {
      return chat.lastMessage.content.length > 50
        ? chat.lastMessage.content.substring(0, 50) + '...'
        : chat.lastMessage.content;
    }
    return 'No messages yet';
  }

  getLastMessageAuthor(chat: Chat): string | null {
    if (!chat.lastMessage) return null;
    return (
      chat.lastMessage.firstAndLastName ||
      chat.lastMessage.chatMember?.nickname ||
      null
    );
  }

  getChatMembersCount(chat: Chat): number {
    return chat.chatMembers?.length || 0;
  }

  formatDate(date: Date): string {
    const now = new Date();
    const chatDate = new Date(date);
    const diffTime = Math.abs(now.getTime() - chatDate.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    if (diffDays === 1) {
      return 'Today';
    } else if (diffDays === 2) {
      return 'Yesterday';
    } else if (diffDays <= 7) {
      return chatDate.toLocaleDateString('en-US', { weekday: 'short' });
    } else {
      return chatDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
    }
  }
}
