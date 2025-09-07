import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface CreateChatMemberRequest {
  nickname: string;
  memberId: string;
  chatId: string;
}

export interface ChatMemberResponse {
  id: string;
  nickname: string;
  memberId: string;
  chat: {
    id: string;
    title: string;
  };
  isInvitationAccepted: boolean;
}

export interface ChatMembersListResponse {
  chatMembers: {
    id: string;
    nickname: string;
    memberId: string;
  }[];
  count: number;
}

@Injectable({
  providedIn: 'root'
})
export class ChatMemberService {
  private http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/chat/api`;

  createChatMember(request: CreateChatMemberRequest): Observable<ChatMemberResponse> {
    return this.http.post<ChatMemberResponse>(`${this.apiUrl}/chat-members`, request);
  }

  getChatMembersByChat(chatId: string, page: number = 0, size: number = 50): Observable<ChatMembersListResponse> {
    return this.http.get<ChatMembersListResponse>(`${this.apiUrl}/chats/${chatId}/chat-members?page=${page}&size=${size}`);
  }

  deleteChatMember(chatMemberId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/chat-members/${chatMemberId}`);
  }
}
