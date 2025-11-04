package com.keepitup.magjob.chat.chatmessage.controller.impl;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chat.service.impl.ChatDefaultService;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import com.keepitup.magjob.chat.chatmember.service.impl.ChatMemberDefaultService;
import com.keepitup.magjob.chat.chatmessage.dto.GetChatMessageResponse;
import com.keepitup.magjob.chat.chatmessage.dto.GetChatMessagesResponse;
import com.keepitup.magjob.chat.chatmessage.dto.PatchChatMessageRequest;
import com.keepitup.magjob.chat.chatmessage.dto.PatchChatMessageWebSocketRequest;
import com.keepitup.magjob.chat.chatmessage.dto.PostChatMessageRequest;
import com.keepitup.magjob.chat.chatmessage.dto.TypingEventRequest;
import com.keepitup.magjob.chat.chatmessage.entity.ChatMessage;
import com.keepitup.magjob.chat.chatmessage.function.ChatMessagesToResponseFunction;
import com.keepitup.magjob.chat.chatmessage.function.RequestToChatMessageFunction;
import com.keepitup.magjob.chat.chatmessage.function.UpdateChatMessageWithRequestFunction;
import com.keepitup.magjob.chat.chatmessage.service.impl.ChatMessageDefaultService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageImpl;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpStatus;
import org.springframework.web.server.ResponseStatusException;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class ChatMessageDefaultControllerTest {

    @Mock
    private ChatMessageDefaultService chatMessageService;

    @Mock
    private ChatDefaultService chatService;

    @Mock
    private ChatMemberDefaultService chatMemberService;

    @Mock
    private RequestToChatMessageFunction requestToChatMessageFunction;

    @Mock
    private UpdateChatMessageWithRequestFunction updateChatMessageWithRequestFunction;

    @Mock
    private ChatMessagesToResponseFunction chatMessagesToResponseFunction;

    @InjectMocks
    private ChatMessageDefaultController chatMessageController;

    private Chat chat;
    private ChatMember chatMember;
    private ChatMessage chatMessage;
    private UUID chatId;
    private UUID memberId;
    private UUID chatMemberId;
    private UUID messageId;
    private UUID organizationId;

    @BeforeEach
    void setUp() {
        chatId = UUID.randomUUID();
        memberId = UUID.randomUUID();
        chatMemberId = UUID.randomUUID();
        messageId = UUID.randomUUID();
        organizationId = UUID.randomUUID();

        chat = Chat.builder()
                .id(chatId)
                .title("Test Chat")
                .organizationId(organizationId)
                .dateOfCreation(LocalDate.now())
                .chatMembers(new ArrayList<>())
                .build();

        chatMember = ChatMember.builder()
                .id(chatMemberId)
                .chat(chat)
                .memberId(memberId)
                .nickname("Test User")
                .chatMessages(new ArrayList<>())
                .build();

        chatMessage = ChatMessage.builder()
                .id(messageId)
                .content("Test message")
                .chat(chat)
                .chatMember(chatMember)
                .dateOfCreation(LocalDateTime.now())
                .viewedBy(new ArrayList<>())
                .firstAndLastName("Test User")
                .build();
    }

    @Test
    void testGetChatMessagesByChat() {
        int page = 0;
        int size = 10;
        PageRequest pageRequest = PageRequest.of(page, size, Sort.by(Sort.Direction.ASC, "dateOfCreation"));
        Page<ChatMessage> messagePage = new PageImpl<>(List.of(chatMessage), pageRequest, 1);
        GetChatMessagesResponse response = GetChatMessagesResponse.builder()
                .chatMessages(List.of())
                .count(1)
                .build();

        when(chatService.find(chatId)).thenReturn(Optional.of(chat));
        when(chatMessageService.findAllByChat(chat, Pageable.unpaged()))
                .thenReturn(new PageImpl<>(List.of(chatMessage)));
        when(chatMessageService.findAllByChat(chat, pageRequest))
                .thenReturn(messagePage);
        when(chatMessagesToResponseFunction.apply(messagePage, 1)).thenReturn(response);

        GetChatMessagesResponse result = chatMessageController.getChatMessagesByChat(page, size, chatId);

        assertNotNull(result);
        verify(chatService).find(chatId);
        verify(chatMessageService).findAllByChat(chat, Pageable.unpaged());
        verify(chatMessageService).findAllByChat(chat, pageRequest);
        verify(chatMessagesToResponseFunction).apply(messagePage, 1);
    }

    @Test
    void testGetChatMessagesByChat_ChatNotFound() {
        int page = 0;
        int size = 10;

        when(chatService.find(chatId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMessageController.getChatMessagesByChat(page, size, chatId);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatService).find(chatId);
        verify(chatMessageService, never()).findAllByChat(any(), any());
    }

    @Test
    void testSendMessage() {
        PostChatMessageRequest request = PostChatMessageRequest.builder()
                .content("New message")
                .chat(chatId)
                .chatMember(chatMemberId)
                .firstAndLastName("Test User")
                .build();

        ChatMessage newMessage = ChatMessage.builder()
                .id(messageId)
                .content("New message")
                .chat(chat)
                .chatMember(chatMember)
                .dateOfCreation(LocalDateTime.now())
                .viewedBy(new ArrayList<>())
                .firstAndLastName("Test User")
                .build();

        when(chatService.find(chatId)).thenReturn(Optional.of(chat));
        when(chatMemberService.find(chatMemberId)).thenReturn(Optional.of(chatMember));
        when(requestToChatMessageFunction.apply(request)).thenReturn(newMessage);
        when(chatMessageService.create(newMessage)).thenReturn(newMessage);

        GetChatMessageResponse result = chatMessageController.sendMessage(chatId, request);

        assertNotNull(result);
        assertEquals(messageId, result.getId());
        assertEquals("New message", result.getContent());
        verify(chatService).find(chatId);
        verify(chatMemberService).find(chatMemberId);
        verify(chatMessageService).create(newMessage);
    }

    @Test
    void testSendMessage_ChatNotFound() {
        PostChatMessageRequest request = PostChatMessageRequest.builder()
                .content("New message")
                .chat(chatId)
                .chatMember(chatMemberId)
                .build();

        when(chatService.find(chatId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMessageController.sendMessage(chatId, request);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatService).find(chatId);
        verify(chatMessageService, never()).create(any());
    }

    @Test
    void testSendMessage_ChatMemberNotFound() {
        PostChatMessageRequest request = PostChatMessageRequest.builder()
                .content("New message")
                .chat(chatId)
                .chatMember(chatMemberId)
                .build();

        when(chatService.find(chatId)).thenReturn(Optional.of(chat));
        when(chatMemberService.find(chatMemberId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMessageController.sendMessage(chatId, request);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatService).find(chatId);
        verify(chatMemberService).find(chatMemberId);
        verify(chatMessageService, never()).create(any());
    }

    @Test
    void testMarkMessageAsViewed() {
        PatchChatMessageRequest request = PatchChatMessageRequest.builder()
                .viewedBy("user1")
                .build();

        ChatMessage updatedMessage = ChatMessage.builder()
                .id(messageId)
                .content("Test message")
                .chat(chat)
                .chatMember(chatMember)
                .viewedBy(List.of("user1"))
                .build();

        when(chatMessageService.find(messageId)).thenReturn(Optional.of(chatMessage));
        when(chatService.find(messageId)).thenReturn(Optional.of(chat));
        when(updateChatMessageWithRequestFunction.apply(chatMessage, request)).thenReturn(updatedMessage);

        chatMessageController.markMessageAsViewed(messageId, request);

        verify(chatMessageService).find(messageId);
        verify(chatService).find(messageId);
        verify(chatMessageService).update(updatedMessage);
    }

    @Test
    void testMarkMessageAsViewed_MessageNotFound() {
        PatchChatMessageRequest request = PatchChatMessageRequest.builder()
                .viewedBy("user1")
                .build();

        when(chatMessageService.find(messageId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMessageController.markMessageAsViewed(messageId, request);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatMessageService).find(messageId);
        verify(chatMessageService, never()).update(any());
    }

    @Test
    void testMarkMessageAsViewed_ChatNotFound() {
        PatchChatMessageRequest request = PatchChatMessageRequest.builder()
                .viewedBy("user1")
                .build();

        when(chatMessageService.find(messageId)).thenReturn(Optional.of(chatMessage));
        when(chatService.find(messageId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMessageController.markMessageAsViewed(messageId, request);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatMessageService).find(messageId);
        verify(chatService).find(messageId);
        verify(chatMessageService, never()).update(any());
    }

    @Test
    void testHandleViewedMessage() {
        PatchChatMessageWebSocketRequest request = PatchChatMessageWebSocketRequest.builder()
                .chatMessageId(messageId)
                .viewedBy("user1")
                .build();

        ChatMessage updatedMessage = ChatMessage.builder()
                .id(messageId)
                .content("Test message")
                .chat(chat)
                .chatMember(chatMember)
                .viewedBy(List.of("user1"))
                .build();

        when(chatMessageService.find(messageId)).thenReturn(Optional.of(chatMessage));
        when(chatService.find(chatId)).thenReturn(Optional.of(chat));
        when(updateChatMessageWithRequestFunction.apply(any(), any())).thenReturn(updatedMessage);

        chatMessageController.handleViewedMessage(chatId, request);

        verify(chatMessageService).find(messageId);
        verify(chatService).find(chatId);
        verify(chatMessageService).update(updatedMessage);
    }

    @Test
    void testHandleViewedMessage_MessageNotFound() {
        PatchChatMessageWebSocketRequest request = PatchChatMessageWebSocketRequest.builder()
                .chatMessageId(messageId)
                .viewedBy("user1")
                .build();

        when(chatMessageService.find(messageId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMessageController.handleViewedMessage(chatId, request);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatMessageService).find(messageId);
        verify(chatMessageService, never()).update(any());
    }

    @Test
    void testHandleViewedMessage_ChatNotFound() {
        PatchChatMessageWebSocketRequest request = PatchChatMessageWebSocketRequest.builder()
                .chatMessageId(messageId)
                .viewedBy("user1")
                .build();

        when(chatMessageService.find(messageId)).thenReturn(Optional.of(chatMessage));
        when(chatService.find(chatId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMessageController.handleViewedMessage(chatId, request);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatMessageService).find(messageId);
        verify(chatService).find(chatId);
        verify(chatMessageService, never()).update(any());
    }

    @Test
    void testHandleTypingEvent() {
        TypingEventRequest request = TypingEventRequest.builder()
                .type("typing")
                .memberId(memberId)
                .memberName("Test User")
                .timestamp(LocalDateTime.now().toString())
                .build();

        when(chatService.find(chatId)).thenReturn(Optional.of(chat));

        TypingEventRequest result = chatMessageController.handleTypingEvent(chatId, request);

        assertNotNull(result);
        assertEquals("typing", result.getType());
        verify(chatService).find(chatId);
    }

    @Test
    void testHandleTypingEvent_ChatNotFound() {
        TypingEventRequest request = TypingEventRequest.builder()
                .type("typing")
                .memberId(memberId)
                .memberName("Test User")
                .timestamp(LocalDateTime.now().toString())
                .build();

        when(chatService.find(chatId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMessageController.handleTypingEvent(chatId, request);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatService).find(chatId);
    }
}

