package com.keepitup.magjob.chat.chatmember.controller.impl;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chat.service.api.ChatService;
import com.keepitup.magjob.chat.chatmember.dto.GetChatMemberResponse;
import com.keepitup.magjob.chat.chatmember.dto.GetChatMembersResponse;
import com.keepitup.magjob.chat.chatmember.dto.PatchChatMemberRequest;
import com.keepitup.magjob.chat.chatmember.dto.PostChatMemberRequest;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import com.keepitup.magjob.chat.chatmember.function.ChatMemberToResponseFunction;
import com.keepitup.magjob.chat.chatmember.function.ChatMembersToResponseFunction;
import com.keepitup.magjob.chat.chatmember.function.RequestToChatMemberFunction;
import com.keepitup.magjob.chat.chatmember.function.UpdateChatMemberWithRequestFunction;
import com.keepitup.magjob.chat.chatmember.service.api.ChatMemberService;
import com.keepitup.magjob.chat.configuration.Constants;
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
import org.springframework.messaging.simp.SimpMessagingTemplate;
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
class ChatMemberDefaultControllerTest {

    @Mock
    private ChatMemberService chatMemberService;

    @Mock
    private ChatService chatService;

    @Mock
    private SimpMessagingTemplate messagingTemplate;

    @Mock
    private ChatMemberToResponseFunction chatMemberToResponseFunction;

    @Mock
    private ChatMembersToResponseFunction chatMembersToResponseFunction;

    @Mock
    private RequestToChatMemberFunction requestToChatMemberFunction;

    @Mock
    private UpdateChatMemberWithRequestFunction updateChatMemberWithRequestFunction;

    @InjectMocks
    private ChatMemberDefaultController chatMemberController;

    private Chat chat;
    private ChatMember chatMember;
    private UUID chatId;
    private UUID memberId;
    private UUID chatMemberId;
    private UUID organizationId;

    @BeforeEach
    void setUp() {
        chatId = UUID.randomUUID();
        memberId = UUID.randomUUID();
        chatMemberId = UUID.randomUUID();
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
    }

    @Test
    void testGetChatMembersByMember() {
        int page = 0;
        int size = 10;
        PageRequest pageRequest = PageRequest.of(page, size);
        Page<ChatMember> chatMemberPage = new PageImpl<>(List.of(chatMember), pageRequest, 1);
        GetChatMembersResponse response = GetChatMembersResponse.builder()
                .chatMembers(List.of())
                .count(1)
                .build();

        when(chatMemberService.findAllByMemberId(memberId, Pageable.unpaged()))
                .thenReturn(new PageImpl<>(List.of(chatMember)));
        when(chatMemberService.findAllByMemberId(memberId, pageRequest))
                .thenReturn(chatMemberPage);
        when(chatMembersToResponseFunction.apply(chatMemberPage, 1)).thenReturn(response);

        GetChatMembersResponse result = chatMemberController.getChatMembersByMember(page, size, memberId);

        assertNotNull(result);
        verify(chatMemberService).findAllByMemberId(memberId, Pageable.unpaged());
        verify(chatMemberService).findAllByMemberId(memberId, pageRequest);
        verify(chatMembersToResponseFunction).apply(chatMemberPage, 1);
    }

    @Test
    void testGetChatMembersByChat() {
        int page = 0;
        int size = 10;
        PageRequest pageRequest = PageRequest.of(page, size);
        Page<ChatMember> chatMemberPage = new PageImpl<>(List.of(chatMember), pageRequest, 1);
        GetChatMembersResponse response = GetChatMembersResponse.builder()
                .chatMembers(List.of())
                .count(1)
                .build();

        when(chatService.find(chatId)).thenReturn(Optional.of(chat));
        when(chatMemberService.findAllByChat(chat, Pageable.unpaged()))
                .thenReturn(new PageImpl<>(List.of(chatMember)));
        when(chatMemberService.findAllByChat(chat, pageRequest))
                .thenReturn(chatMemberPage);
        when(chatMembersToResponseFunction.apply(chatMemberPage, 1)).thenReturn(response);

        GetChatMembersResponse result = chatMemberController.getChatMembersByChat(page, size, chatId);

        assertNotNull(result);
        verify(chatService).find(chatId);
        verify(chatMemberService).findAllByChat(chat, Pageable.unpaged());
        verify(chatMemberService).findAllByChat(chat, pageRequest);
    }

    @Test
    void testGetChatMembersByChat_NotFound() {
        int page = 0;
        int size = 10;

        when(chatService.find(chatId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMemberController.getChatMembersByChat(page, size, chatId);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatService).find(chatId);
    }

    @Test
    void testCreateChatMember() {
        PostChatMemberRequest request = PostChatMemberRequest.builder()
                .chatId(chatId)
                .memberId(memberId)
                .nickname("New Member")
                .build();

        ChatMember newChatMember = ChatMember.builder()
                .id(chatMemberId)
                .chat(chat)
                .memberId(memberId)
                .nickname("New Member")
                .build();

        GetChatMemberResponse response = GetChatMemberResponse.builder()
                .id(chatMemberId)
                .nickname("New Member")
                .build();

        when(chatService.find(chatId)).thenReturn(Optional.of(chat));
        when(requestToChatMemberFunction.apply(request)).thenReturn(newChatMember);
        when(chatMemberService.findByMemberIdAndChat(memberId, chat))
                .thenReturn(Optional.of(newChatMember));
        when(chatMemberToResponseFunction.apply(newChatMember)).thenReturn(response);

        GetChatMemberResponse result = chatMemberController.createChatMember(request);

        assertNotNull(result);
        verify(chatService).find(chatId);
        verify(chatMemberService).create(newChatMember);
        verify(chatMemberService).findByMemberIdAndChat(memberId, chat);
    }

    @Test
    void testCreateChatMember_ChatNotFound() {
        PostChatMemberRequest request = PostChatMemberRequest.builder()
                .chatId(chatId)
                .memberId(memberId)
                .nickname("New Member")
                .build();

        when(chatService.find(chatId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMemberController.createChatMember(request);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatService).find(chatId);
        verify(chatMemberService, never()).create(any());
    }

    @Test
    void testSetNickname() {
        PatchChatMemberRequest request = PatchChatMemberRequest.builder()
                .nickname("Updated Nickname")
                .build();

        ChatMember updatedChatMember = ChatMember.builder()
                .id(chatMemberId)
                .chat(chat)
                .memberId(memberId)
                .nickname("Updated Nickname")
                .build();

        GetChatMemberResponse response = GetChatMemberResponse.builder()
                .id(chatMemberId)
                .nickname("Updated Nickname")
                .build();

        when(chatMemberService.find(chatMemberId)).thenReturn(Optional.of(chatMember));
        when(updateChatMemberWithRequestFunction.apply(any(ChatMember.class), eq(request))).thenReturn(updatedChatMember);
        when(chatMemberService.find(chatMemberId)).thenReturn(Optional.of(updatedChatMember));
        when(chatMemberToResponseFunction.apply(updatedChatMember)).thenReturn(response);

        GetChatMemberResponse result = chatMemberController.setNickname(chatMemberId, request);

        assertNotNull(result);
        verify(chatMemberService).update(updatedChatMember);
        verify(chatMemberService, times(2)).find(chatMemberId);
    }

    @Test
    void testSetNickname_NotFound() {
        PatchChatMemberRequest request = PatchChatMemberRequest.builder()
                .nickname("Updated Nickname")
                .build();

        when(chatMemberService.find(chatMemberId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMemberController.setNickname(chatMemberId, request);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatMemberService).find(chatMemberId);
        verify(chatMemberService, never()).update(any());
    }

    @Test
    void testDeleteChatMember() {
        when(chatMemberService.find(chatMemberId)).thenReturn(Optional.of(chatMember));

        chatMemberController.deleteChatMember(chatMemberId);

        verify(chatMemberService).find(chatMemberId);
        verify(chatMemberService).delete(chatMemberId);
    }

    @Test
    void testDeleteChatMember_NotFound() {
        when(chatMemberService.find(chatMemberId)).thenReturn(Optional.empty());

        ResponseStatusException exception = assertThrows(ResponseStatusException.class, () -> {
            chatMemberController.deleteChatMember(chatMemberId);
        });

        assertEquals(HttpStatus.NOT_FOUND, exception.getStatusCode());
        verify(chatMemberService).find(chatMemberId);
        verify(chatMemberService, never()).delete(any());
    }
}

