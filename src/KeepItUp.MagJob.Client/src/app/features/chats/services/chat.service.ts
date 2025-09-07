import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import {
  Chat,
  ChatMessage,
  CreateChatRequest,
  SendMessageRequest,
  ChatListResponse,
  ChatMessagesResponse,
} from '../models/chat.model';
import { WebSocketService } from './websocket.service';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private http = inject(HttpClient);
  private webSocketService = inject(WebSocketService);

  private readonly apiUrl = `${environment.apiUrl}/api/chat/api`;
  private readonly wsUrl = `${environment.apiUrl}/ws`;

  private _chatsSubject = new BehaviorSubject<Chat[]>([]);
  private selectedChatSubject = new BehaviorSubject<Chat | null>(null);
  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);

  public chats$ = this._chatsSubject.asObservable();
  public selectedChat$ = this.selectedChatSubject.asObservable();
  public messages$ = this.messagesSubject.asObservable();

  public get chatsSubject() {
    return this._chatsSubject;
  }

  constructor() {
    void this.initializeWebSocket();
  }

  private async initializeWebSocket(): Promise<void> {
    try {
      await this.webSocketService.connect(this.wsUrl);
      console.log('WebSocket connected successfully');
    } catch (error) {
      console.error('Failed to connect to WebSocket:', error);
    }
  }

  getChatsByMemberId(memberId: string): Observable<Chat[]> {
    return this.http
      .get<ChatListResponse>(`${this.apiUrl}/members/${memberId}/chats`)
      .pipe(map(response => response.chats || []));
  }

  getChatMessages(chatId: string, page = 0, size = 50): Observable<ChatMessagesResponse> {
    return this.http.get<ChatMessagesResponse>(
      `${this.apiUrl}/chats/${chatId}/chat-messages?page=${page}&size=${size}`,
    );
  }

  createChat(request: CreateChatRequest): Observable<Chat> {
    return this.http.post<Chat>(`${this.apiUrl}/chats`, request);
  }

  sendMessage(request: SendMessageRequest): void {
    const backendRequest = {
      content: request.content,
      chat: request.chatId,
      chatMember: request.chatMemberId,
      attachment: request.attachment,
    };

    console.log('ChatService sending message:', {
      originalRequest: request,
      backendRequest: backendRequest,
      destination: `/app/chat/${request.chatId}/sendMessage`,
    });

    this.webSocketService.send(`/app/chat/${request.chatId}/sendMessage`, backendRequest);
  }

  joinChat(chatId: string): void {
    console.log('Joining chat:', chatId, 'subscribing to:', `/topic/chat/${chatId}`);
    try {
      this.webSocketService.subscribe(`/topic/chat/${chatId}`).subscribe({
        next: message => {
          try {
            console.log('Raw WebSocket message received:', message);
            const chatMessage = JSON.parse(message.body) as ChatMessage;
            console.log('Parsed message via WebSocket:', chatMessage);
            this.addMessage(chatMessage);
          } catch (error) {
            console.error('Error parsing WebSocket message:', error);
          }
        },
        error: error => {
          console.error('WebSocket subscription error:', error);
        },
      });
    } catch {
      console.log('Already subscribed to chat:', chatId);
    }
  }

  leaveChat(chatId: string): void {
    this.webSocketService.unsubscribe(`/topic/chat/${chatId}`);
  }

  setChats(chats: Chat[]): void {
    this._chatsSubject.next(chats);
  }

  removeChat(chatId: string): void {
    const currentChats = this._chatsSubject.value;
    const updatedChats = currentChats.filter(chat => chat.id !== chatId);
    this._chatsSubject.next(updatedChats);

    const selectedChat = this.selectedChatSubject.value;
    if (selectedChat && selectedChat.id === chatId) {
      this.setSelectedChat(null);
    }
  }

  setSelectedChat(chat: Chat | null): void {
    const currentChat = this.selectedChatSubject.value;
    if (currentChat && currentChat.id !== chat?.id) {
      this.leaveChat(currentChat.id);
      this.messagesSubject.next([]);
    }

    this.selectedChatSubject.next(chat);
    if (chat) {
      this.joinChat(chat.id);
      this.loadChatMessages(chat.id);
    }
  }

  addMessage(message: ChatMessage): void {
    const currentMessages = this.messagesSubject.value;
    this.messagesSubject.next([...currentMessages, message]);
  }

  private loadChatMessages(chatId: string): void {
    this.getChatMessages(chatId).subscribe({
      next: response => {
        this.messagesSubject.next(response.chatMessages || []);
      },
      error: error => {
        console.error('Error loading chat messages:', error);
      },
    });
  }

  disconnect(): void {
    this.webSocketService.disconnect();
  }
}
