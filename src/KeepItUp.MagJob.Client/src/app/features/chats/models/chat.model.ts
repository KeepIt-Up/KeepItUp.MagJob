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
  isInvitationAccepted: boolean;
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
  viewedBy: string[];
  attachment?: string;
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
}

export interface SendMessageRequest {
  content: string;
  chatId: string;
  chatMemberId: string;
  attachment?: File;
}

export interface ChatListResponse {
  chats: Chat[];
  count: number;
}

export interface ChatMessagesResponse {
  chatMessages: ChatMessage[];
  count: number;
}
