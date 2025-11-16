package com.keepitup.magjob.chat.chat.controller.impl;

import com.keepitup.magjob.chat.chat.dto.GetChatResponse;
import com.keepitup.magjob.chat.chat.dto.GetChatsResponse;
import com.keepitup.magjob.chat.chat.dto.PatchChatRequest;
import com.keepitup.magjob.chat.chat.dto.PostChatRequest;
import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chat.function.ChatToResponseFunction;
import com.keepitup.magjob.chat.chat.function.ChatsToResponseFunction;
import com.keepitup.magjob.chat.chat.function.RequestToChatFunction;
import com.keepitup.magjob.chat.chat.function.UpdateChatWithRequestFunction;
import com.keepitup.magjob.chat.chat.service.impl.ChatDefaultService;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import com.keepitup.magjob.chat.chatmember.service.api.ChatMemberService;
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
import org.springframework.http.HttpStatus;
import org.springframework.web.server.ResponseStatusException;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class ChatDefaultControllerTest {

    @Mock
    private ChatDefaultService chatService;

    @Mock
    private ChatMemberService chatMemberService;

    @Mock
    private ChatToResponseFunction chatToResponseFunction;

    @Mock
    private ChatsToResponseFunction chatsToResponseFunction;

    @Mock
    private RequestToChatFunction requestToChatFunction;

    @Mock
    private UpdateChatWithRequestFunction updateChatWithRequestFunction;

    @InjectMocks
    private ChatDefaultController chatController;

    private Chat chat;
    private UUID chatId;
    private UUID organizationId;
    private UUID memberId;
    private GetChatResponse getChatResponse;
    private GetChatsResponse getChatsResponse;

    @BeforeEach
    void setUp() {
        chatId = UUID.randomUUID();
        organizationId = UUID.randomUUID();
        memberId = UUID.randomUUID();

        chat = Chat.builder()
                .id(chatId)
                .title("Test Chat")
                .organizationId(organizationId)
                .dateOfCreation(LocalDate.now())
                .chatMembers(new ArrayList<>())
                .build();

        getChatResponse = GetChatResponse.builder()
                .id(chatId)
                .title("Test Chat")
                .organizationId(organizationId)
                .dateOfCreation(LocalDate.now())
                .build();

        GetChatsResponse.Chat chatResponse = GetChatsResponse.Chat.builder()
                .id(chatId)
                .title("Test Chat")
                .organizationId(organizationId)
                .build();
        
        getChatsResponse = GetChatsResponse.builder()
                .chats(List.of(chatResponse))
                .count(1)
                .build();
    }

    @Test
    void testGetChats() {
        int page = 0;
        int size = 10;
        PageRequest pageRequest = PageRequest.of(page, size);
        Page<Chat> chatPage = new PageImpl<>(List.of(chat), pageRequest, 1);

        when(chatService.findAll()).thenReturn(List.of(chat));
        when(chatService.findAll(pageRequest)).thenReturn(chatPage);
        when(chatsToResponseFunction.apply(chatPage, 1)).thenReturn(getChatsResponse);

        GetChatsResponse result = chatController.getChats(page, size);

        assertNotNull(result);
        verify(chatService).findAll();
        verify(chatService).findAll(pageRequest);
        verify(chatsToResponseFunction).apply(chatPage, 1);
    }

    @Test
    void testGetChat_Success() {
        when(chatService.find(chatId)).thenReturn(Optional.of(chat));
        when(chatToResponseFunction.apply(chat)).thenReturn(getChatResponse);

        GetChatResponse result = chatController.getChat(chatId);

        assertNotNull(result);
        assertEquals(chatId, result.getId());
        verify(chatService).find(chatId);
        verify(chatToResponseFunction).apply(chat);
    }

    @Test
    void testGetChat_NotFound() {
        when(chatService.find(chatId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatController.getChat(chatId);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatService).find(chatId);
        verify(chatToResponseFunction, never()).apply(any());
    }

    @Test
    void testGetChatsByOrganization() {
        int page = 0;
        int size = 10;
        PageRequest pageRequest = PageRequest.of(page, size);
        Page<Chat> chatPage = new PageImpl<>(List.of(chat), pageRequest, 1);

        when(chatService.findAllByOrganizationId(organizationId, Pageable.unpaged()))
                .thenReturn(new PageImpl<>(List.of(chat)));
        when(chatService.findAllByOrganizationId(organizationId, pageRequest))
                .thenReturn(chatPage);
        when(chatsToResponseFunction.apply(chatPage, 1)).thenReturn(getChatsResponse);

        GetChatsResponse result = chatController.getChatsByOrganization(page, size, organizationId);

        assertNotNull(result);
        verify(chatService).findAllByOrganizationId(organizationId, Pageable.unpaged());
        verify(chatService).findAllByOrganizationId(organizationId, pageRequest);
        verify(chatsToResponseFunction).apply(chatPage, 1);
    }

    @Test
    void testGetChatsByMember() {
        int page = 0;
        int size = 10;
        PageRequest pageRequest = PageRequest.of(page, size);
        Page<Chat> chatPage = new PageImpl<>(List.of(chat), pageRequest, 1);

        when(chatMemberService.findAllChatsByMemberId(memberId, Pageable.unpaged()))
                .thenReturn(new PageImpl<>(List.of(chat)));
        when(chatMemberService.findAllChatsByMemberId(memberId, pageRequest))
                .thenReturn(chatPage);
        when(chatsToResponseFunction.apply(chatPage, 1)).thenReturn(getChatsResponse);

        GetChatsResponse result = chatController.getChatsByMember(page, size, memberId);

        assertNotNull(result);
        verify(chatMemberService).findAllChatsByMemberId(memberId, Pageable.unpaged());
        verify(chatMemberService).findAllChatsByMemberId(memberId, pageRequest);
        verify(chatsToResponseFunction).apply(chatPage, 1);
    }

    @Test
    void testCreateChat() {
        PostChatRequest postChatRequest = PostChatRequest.builder()
                .title("New Chat")
                .organizationId(organizationId)
                .memberId(memberId)
                .nickname("Test User")
                .build();

        Chat createdChat = Chat.builder()
                .id(chatId)
                .title("New Chat")
                .organizationId(organizationId)
                .build();

        when(requestToChatFunction.apply(postChatRequest)).thenReturn(createdChat);
        when(chatService.findByTitle("New Chat")).thenReturn(Optional.of(createdChat));
        when(chatToResponseFunction.apply(createdChat)).thenReturn(getChatResponse);

        GetChatResponse result = chatController.createChat(postChatRequest);

        assertNotNull(result);
        verify(chatService).create(createdChat);
        verify(chatMemberService).create(any(ChatMember.class));
        verify(chatService, times(2)).findByTitle("New Chat");
    }

    @Test
    void testUpdateChat() {
        PatchChatRequest patchChatRequest = PatchChatRequest.builder()
                .title("Updated Chat")
                .build();

        Chat updatedChat = Chat.builder()
                .id(chatId)
                .title("Updated Chat")
                .organizationId(organizationId)
                .build();

        GetChatResponse updatedResponse = GetChatResponse.builder()
                .id(chatId)
                .title("Updated Chat")
                .organizationId(organizationId)
                .build();

        when(chatService.find(chatId)).thenReturn(Optional.of(chat));
        when(updateChatWithRequestFunction.apply(any(Chat.class), eq(patchChatRequest))).thenReturn(updatedChat);
        when(chatService.find(chatId)).thenReturn(Optional.of(updatedChat));
        when(chatToResponseFunction.apply(updatedChat)).thenReturn(updatedResponse);

        GetChatResponse result = chatController.updateChat(chatId, patchChatRequest);

        assertNotNull(result);
        verify(chatService).update(updatedChat);
        verify(chatService, times(2)).find(chatId);
    }

    @Test
    void testUpdateChat_NotFound() {
        PatchChatRequest patchChatRequest = PatchChatRequest.builder()
                .title("Updated Chat")
                .build();

        when(chatService.find(chatId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatController.updateChat(chatId, patchChatRequest);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatService).find(chatId);
        verify(chatService, never()).update(any());
    }

    @Test
    void testDeleteChat() {
        when(chatService.find(chatId)).thenReturn(Optional.of(chat));

        chatController.deleteChat(chatId);

        verify(chatService).find(chatId);
        verify(chatService).delete(chatId);
    }

    @Test
    void testDeleteChat_NotFound() {
        when(chatService.find(chatId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatController.deleteChat(chatId);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatService).find(chatId);
        verify(chatService, never()).delete(any());
    }
}

