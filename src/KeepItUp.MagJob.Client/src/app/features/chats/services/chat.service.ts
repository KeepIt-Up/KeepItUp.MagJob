import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, filter, take } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@environments/environment';
import {
  Chat,
  ChatMessage,
  CreateChatRequest,
  SendMessageRequest,
  ChatListResponse,
  ChatMessagesResponse,
  TypingEvent,
  TypingUser,
} from '../models/chat.model';
import { WebSocketService } from './websocket.service';
import { UserContextService } from '../../users/services/user-context.service';
import { NotificationService } from '@shared/services/notification.service';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private http = inject(HttpClient);
  private webSocketService = inject(WebSocketService);
  private userContextService = inject(UserContextService);
  private notificationService = inject(NotificationService);

  private readonly apiUrl = `${environment.apiUrl}/api/chat/api`;
  private readonly wsUrl = `${environment.apiUrl}/ws`;

  private _chatsSubject = new BehaviorSubject<Chat[]>([]);
  private selectedChatSubject = new BehaviorSubject<Chat | null>(null);
  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);
  private typingUsersSubject = new BehaviorSubject<TypingUser[]>([]);
  private notificationSubscriptions = new Set<string>();

  public chats$ = this._chatsSubject.asObservable();
  public selectedChat$ = this.selectedChatSubject.asObservable();
  public messages$ = this.messagesSubject.asObservable();
  public typingUsers$ = this.typingUsersSubject.asObservable();

  public get chatsSubject() {
    return this._chatsSubject;
  }

  constructor() {
    void this.initializeWebSocket();
    this.setupNotificationSubscription();
  }

  private async initializeWebSocket(): Promise<void> {
    try {
      await this.webSocketService.connect(this.wsUrl);
      console.log('WebSocket connected successfully');
    } catch (error) {
      console.error('Failed to connect to WebSocket:', error);
    }
  }

  private setupNotificationSubscription(): void {
    this.userContextService.user$
      .pipe(
        filter(user => user !== null),
        take(1),
      )
      .subscribe(() => {
        this.subscribeToUserNotifications();
      });
  }

  private subscribeToUserNotifications(): void {
    const currentUser = this.userContextService.getCurrentUser();
    const isConnected = this.webSocketService.isConnected();

    if (!isConnected) {
      this.webSocketService.onConnectionChange().subscribe(connected => {
        if (connected) {
          this.subscribeToUserNotifications();
        }
      });
      return;
    }

    if (!currentUser) {
      this.userContextService.user$
        .pipe(
          filter(user => user !== null),
          take(1),
        )
        .subscribe(() => {
          this.subscribeToUserNotifications();
        });
      return;
    }

    currentUser.memberships.forEach(membership => {
      const topic = `/topic/user/${membership.memberId}`;
      if (!this.notificationSubscriptions.has(topic)) {
        this.webSocketService.subscribe(topic).subscribe({
          next: message => {
            try {
              const notification = JSON.parse(message.body);
              this.handleChatNotification(notification);
            } catch (error) {
              console.error('Error parsing notification:', error);
            }
          },
          error: error => {
            console.error('Notification subscription error:', error);
          },
        });
        this.notificationSubscriptions.add(topic);
      }
    });
  }

  private handleChatNotification(notification: {
    chatId: string;
    chatTitle: string;
    organizationId: string;
    message: string;
    senderName: string;
  }): void {
    const currentChat = this.selectedChatSubject.value;
    if (currentChat && currentChat.id === notification.chatId) {
      return;
    }

    this.notificationService.show(
      notification.message,
      'info',
      5000,
      notification.chatId,
      notification.organizationId,
    );
  }

  getChatsByMemberId(memberId: string): Observable<Chat[]> {
    return this.http
      .get<ChatListResponse>(`${this.apiUrl}/members/${memberId}/chats`)
      .pipe(map(response => response.chats || []));
  }

  getChatMessages(chatId: string, page = 0, size = 50): Observable<ChatMessagesResponse> {
    return this.http
      .get<ChatMessagesResponse>(
        `${this.apiUrl}/chats/${chatId}/chat-messages?page=${page}&size=${size}`,
      )
      .pipe(
        map(response => ({
          ...response,
          chatMessages: response.chatMessages.map(msg => ({
            ...msg,
            dateOfCreation: new Date(msg.dateOfCreation),
          })),
        })),
      );
  }

  createChat(request: CreateChatRequest): Observable<Chat> {
    return this.http.post<Chat>(`${this.apiUrl}/chats`, request);
  }

  sendMessage(request: SendMessageRequest): void {
    const currentUser = this.userContextService.getCurrentUser();
    const firstAndLastName = currentUser
      ? `${currentUser.firstName} ${currentUser.lastName}`.trim()
      : undefined;

    const backendRequest = {
      content: request.content,
      chat: request.chatId,
      chatMember: request.chatMemberId,
      firstAndLastName: firstAndLastName,
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
            const rawMessage: any = JSON.parse(message.body);
            console.log(
              'Raw message dateOfCreation:',
              rawMessage.dateOfCreation,
              'type:',
              typeof rawMessage.dateOfCreation,
            );

            let parsedDate: Date;
            if (rawMessage.dateOfCreation) {
              if (Array.isArray(rawMessage.dateOfCreation)) {
                const [year, month, day, hour, minute, second, nanoseconds] =
                  rawMessage.dateOfCreation;
                console.log('Parsing array date:', {
                  year,
                  month,
                  day,
                  hour,
                  minute,
                  second,
                  nanoseconds,
                });

                const milliseconds = Math.floor(nanoseconds / 1000000);

                parsedDate = new Date(year, month - 1, day, hour, minute, second, milliseconds);
                console.log('Parsed date result:', parsedDate);
              } else if (typeof rawMessage.dateOfCreation === 'string') {
                parsedDate = new Date(rawMessage.dateOfCreation);
              } else {
                parsedDate = new Date(rawMessage.dateOfCreation);
              }
            } else {
              parsedDate = new Date();
            }

            const chatMessage: ChatMessage = {
              id: rawMessage.id,
              content: rawMessage.content,
              dateOfCreation: parsedDate,
              firstAndLastName: rawMessage.firstAndLastName,
              chatMember: {
                id: rawMessage.chatMember.id,
                nickname: rawMessage.chatMember.nickname,
                memberId: rawMessage.chatMember.memberId,
                isAdmin: false,
                member: {
                  id: rawMessage.chatMember.memberId,
                  fullName: rawMessage.firstAndLastName,
                  firstName: rawMessage.firstAndLastName?.split(' ')[0] || '',
                  lastName: rawMessage.firstAndLastName?.split(' ').slice(1).join(' ') || '',
                },
              },
              chat: {
                id: rawMessage.chat.id,
                title: rawMessage.chat.title,
                organizationId: rawMessage.chat.organizationId,
              },
            };
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
        const mappedMessages: ChatMessage[] = (response.chatMessages || []).map(msg => ({
          id: msg.id,
          content: msg.content,
          dateOfCreation: new Date(msg.dateOfCreation),
          firstAndLastName: msg.firstAndLastName,
          chatMember: {
            id: msg.chatMember?.id || '',
            nickname: msg.chatMember?.nickname,
            memberId: msg.chatMember?.memberId || '',
            isAdmin: false,
            member: {
              id: msg.chatMember?.memberId || '',
              fullName: msg.firstAndLastName || '',
              firstName: msg.firstAndLastName?.split(' ')[0] || '',
              lastName: msg.firstAndLastName?.split(' ').slice(1).join(' ') || '',
            },
          },
          chat: {
            id: msg.chat?.id || '',
            title: msg.chat?.title || '',
            organizationId: msg.chat?.organizationId || '',
          },
        }));
        this.messagesSubject.next(mappedMessages);
      },
      error: error => {
        console.error('Error loading chat messages:', error);
      },
    });
  }

  disconnect(): void {
    this.webSocketService.disconnect();
  }

  sendTypingStart(chatId: string, memberId: string, memberName: string): void {
    this.webSocketService.sendTypingEvent(chatId, memberId, memberName, 'TYPING_START');
  }

  sendTypingStop(chatId: string, memberId: string, memberName: string): void {
    this.webSocketService.sendTypingEvent(chatId, memberId, memberName, 'TYPING_STOP');
  }

  subscribeToTypingEvents(chatId: string): void {
    console.log('Subscribing to typing events for chat:', chatId);
    this.webSocketService.subscribe(`/topic/chat/${chatId}/typing`).subscribe({
      next: message => {
        try {
          const typingEvent: TypingEvent = JSON.parse(message.body);
          console.log('Typing event received:', typingEvent);
          this.handleTypingEvent(typingEvent);
        } catch (error) {
          console.error('Error parsing typing event:', error);
        }
      },
      error: error => {
        console.error('Typing events subscription error:', error);
      },
    });
  }

  private handleTypingEvent(event: TypingEvent): void {
    const currentTypingUsers = this.typingUsersSubject.value;
    let updatedTypingUsers: TypingUser[];

    if (event.type === 'TYPING_START') {
      const existingUserIndex = currentTypingUsers.findIndex(
        user => user.memberId === event.memberId,
      );
      const newUser: TypingUser = {
        memberId: event.memberId,
        memberName: event.memberName,
        timestamp: new Date(),
      };

      if (existingUserIndex >= 0) {
        updatedTypingUsers = [...currentTypingUsers];
        updatedTypingUsers[existingUserIndex] = newUser;
      } else {
        updatedTypingUsers = [...currentTypingUsers, newUser];
      }
    } else {
      updatedTypingUsers = currentTypingUsers.filter(
        user => user.memberId !== event.memberId,
      );
    }

    this.typingUsersSubject.next(updatedTypingUsers);

    if (event.type === 'TYPING_START') {
      setTimeout(() => {
        const currentUsers = this.typingUsersSubject.value;
        const filteredUsers = currentUsers.filter(
          user => user.memberId !== event.memberId,
        );
        this.typingUsersSubject.next(filteredUsers);
      }, 5000);
    }
  }

  clearTypingUsers(): void {
    this.typingUsersSubject.next([]);
  }
}
