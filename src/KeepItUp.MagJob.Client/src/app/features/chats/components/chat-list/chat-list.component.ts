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

  getChatMembersCount(chat: Chat): number {
    return chat.chatMembers?.length || 0;
  }
}
