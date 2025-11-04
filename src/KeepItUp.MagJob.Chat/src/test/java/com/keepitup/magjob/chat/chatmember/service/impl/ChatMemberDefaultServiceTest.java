package com.keepitup.magjob.chat.chatmember.service.impl;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import com.keepitup.magjob.chat.chatmember.repository.api.ChatMemberRepository;
import com.keepitup.magjob.chat.chatmessage.entity.ChatMessage;
import com.keepitup.magjob.chat.chatmessage.repository.api.ChatMessageRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageImpl;
import org.springframework.data.domain.PageRequest;

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
class ChatMemberDefaultServiceTest {

    @Mock
    private ChatMemberRepository chatMemberRepository;

    @Mock
    private ChatMessageRepository chatMessageRepository;

    @InjectMocks
    private ChatMemberDefaultService chatMemberService;

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
                .build();
    }

    @Test
    void testFindAllByMemberId() {
        PageRequest pageRequest = PageRequest.of(0, 10);
        Page<ChatMember> chatMemberPage = new PageImpl<>(List.of(chatMember), pageRequest, 1);
        when(chatMemberRepository.findAllByMemberId(memberId, pageRequest)).thenReturn(chatMemberPage);

        Page<ChatMember> result = chatMemberService.findAllByMemberId(memberId, pageRequest);

        assertEquals(1, result.getTotalElements());
        assertEquals(memberId, result.getContent().get(0).getMemberId());
        verify(chatMemberRepository).findAllByMemberId(memberId, pageRequest);
    }

    @Test
    void testFindAllByChat() {
        PageRequest pageRequest = PageRequest.of(0, 10);
        Page<ChatMember> chatMemberPage = new PageImpl<>(List.of(chatMember), pageRequest, 1);
        when(chatMemberRepository.findAllByChat(chat, pageRequest)).thenReturn(chatMemberPage);

        Page<ChatMember> result = chatMemberService.findAllByChat(chat, pageRequest);

        assertEquals(1, result.getTotalElements());
        assertEquals(chatId, result.getContent().get(0).getChat().getId());
        verify(chatMemberRepository).findAllByChat(chat, pageRequest);
    }

    @Test
    void testFindAllChatsByMemberId() {
        PageRequest pageRequest = PageRequest.of(0, 10);
        Page<ChatMember> chatMemberPage = new PageImpl<>(List.of(chatMember), pageRequest, 1);
        when(chatMemberRepository.findAllByMemberId(memberId, pageRequest)).thenReturn(chatMemberPage);

        Page<Chat> result = chatMemberService.findAllChatsByMemberId(memberId, pageRequest);

        assertEquals(1, result.getTotalElements());
        assertEquals(chatId, result.getContent().get(0).getId());
        verify(chatMemberRepository).findAllByMemberId(memberId, pageRequest);
    }

    @Test
    void testFind() {
        when(chatMemberRepository.findById(chatMemberId)).thenReturn(Optional.of(chatMember));

        Optional<ChatMember> result = chatMemberService.find(chatMemberId);

        assertTrue(result.isPresent());
        assertEquals(chatMemberId, result.get().getId());
        verify(chatMemberRepository).findById(chatMemberId);
    }

    @Test
    void testFind_NotFound() {
        when(chatMemberRepository.findById(chatMemberId)).thenReturn(Optional.empty());

        Optional<ChatMember> result = chatMemberService.find(chatMemberId);

        assertFalse(result.isPresent());
        verify(chatMemberRepository).findById(chatMemberId);
    }

    @Test
    void testFindByMemberIdAndChat() {
        when(chatMemberRepository.findByMemberIdAndChat(memberId, chat)).thenReturn(Optional.of(chatMember));

        Optional<ChatMember> result = chatMemberService.findByMemberIdAndChat(memberId, chat);

        assertTrue(result.isPresent());
        assertEquals(memberId, result.get().getMemberId());
        assertEquals(chatId, result.get().getChat().getId());
        verify(chatMemberRepository).findByMemberIdAndChat(memberId, chat);
    }

    @Test
    void testFindByMemberIdAndChat_NotFound() {
        when(chatMemberRepository.findByMemberIdAndChat(memberId, chat)).thenReturn(Optional.empty());

        Optional<ChatMember> result = chatMemberService.findByMemberIdAndChat(memberId, chat);

        assertFalse(result.isPresent());
        verify(chatMemberRepository).findByMemberIdAndChat(memberId, chat);
    }

    @Test
    void testCreate() {
        when(chatMemberRepository.save(chatMember)).thenReturn(chatMember);

        chatMemberService.create(chatMember);

        verify(chatMemberRepository).save(chatMember);
    }

    @Test
    void testDelete() {
        when(chatMemberRepository.findById(chatMemberId)).thenReturn(Optional.of(chatMember));

        chatMemberService.delete(chatMemberId);

        verify(chatMemberRepository).findById(chatMemberId);
        verify(chatMemberRepository).delete(chatMember);
    }

    @Test
    void testDelete_NotFound() {
        when(chatMemberRepository.findById(chatMemberId)).thenReturn(Optional.empty());

        chatMemberService.delete(chatMemberId);

        verify(chatMemberRepository).findById(chatMemberId);
        verify(chatMemberRepository, never()).delete(any());
    }

    @Test
    void testDelete_WithMessages() {
        List<ChatMessage> messages = new ArrayList<>();
        messages.add(chatMessage);
        chatMember.setChatMessages(messages);
        chatMessage.setChatMember(chatMember);

        when(chatMemberRepository.findById(chatMemberId)).thenReturn(Optional.of(chatMember));
        when(chatMessageRepository.saveAll(messages)).thenReturn(messages);

        chatMemberService.delete(chatMemberId);

        assertNull(chatMessage.getChatMember());
        verify(chatMemberRepository).findById(chatMemberId);
        verify(chatMessageRepository).saveAll(messages);
        verify(chatMemberRepository).delete(chatMember);
    }

    @Test
    void testDelete_WithNullMessages() {
        chatMember.setChatMessages(null);

        when(chatMemberRepository.findById(chatMemberId)).thenReturn(Optional.of(chatMember));

        chatMemberService.delete(chatMemberId);

        verify(chatMemberRepository).findById(chatMemberId);
        verify(chatMessageRepository, never()).saveAll(any());
        verify(chatMemberRepository).delete(chatMember);
    }

    @Test
    void testDelete_WithEmptyMessages() {
        chatMember.setChatMessages(new ArrayList<>());

        when(chatMemberRepository.findById(chatMemberId)).thenReturn(Optional.of(chatMember));

        chatMemberService.delete(chatMemberId);

        verify(chatMemberRepository).findById(chatMemberId);
        verify(chatMessageRepository, never()).saveAll(any());
        verify(chatMemberRepository).delete(chatMember);
    }

    @Test
    void testUpdate() {
        when(chatMemberRepository.save(chatMember)).thenReturn(chatMember);

        chatMemberService.update(chatMember);

        verify(chatMemberRepository).save(chatMember);
    }
}

