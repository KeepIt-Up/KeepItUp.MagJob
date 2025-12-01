export interface Chat {
  id: string;
  title: string;
  dateOfCreation: Date;
  organizationId: string;
  chatMembers?: ChatMember[];
  lastMessage?: ChatMessage;
}

export interface ChatMember {
  id: string;
  nickname?: string;
  memberId: string;
  isAdmin: boolean;
  member?: {
    id: string;
    fullName: string;
    firstName: string;
    lastName: string;
  };
}

export interface ChatMessage {
  id: string;
  content: string;
  dateOfCreation: Date;
  firstAndLastName: string;
  chatMember: ChatMember;
  chat: {
    id: string;
    title: string;
    organizationId: string;
  };
}

export interface CreateChatRequest {
  title: string;
  organizationId: string;
  memberId: string;
  nickname: string;
}

export interface SendMessageRequest {
  content: string;
  chatId: string;
  chatMemberId: string;
}

export interface ChatListResponse {
  chats: Chat[];
  count: number;
}

export interface ChatMessagesResponse {
  chatMessages: ChatMessage[];
  count: number;
}

export interface TypingEvent {
  type: 'TYPING_START' | 'TYPING_STOP';
  chatId: string;
  memberId: string;
  memberName: string;
}

export interface TypingUser {
  memberId: string;
  memberName: string;
  timestamp: Date;
}