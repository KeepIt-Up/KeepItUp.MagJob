package com.keepitup.magjob.chat.chatmessage.service.impl;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
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
class ChatMessageDefaultServiceTest {

    @Mock
    private ChatMessageRepository chatMessageRepository;

    @InjectMocks
    private ChatMessageDefaultService chatMessageService;

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
                .build();
    }

    @Test
    void testFind() {
        when(chatMessageRepository.findById(messageId)).thenReturn(Optional.of(chatMessage));

        Optional<ChatMessage> result = chatMessageService.find(messageId);

        assertTrue(result.isPresent());
        assertEquals(messageId, result.get().getId());
        verify(chatMessageRepository).findById(messageId);
    }

    @Test
    void testFind_NotFound() {
        when(chatMessageRepository.findById(messageId)).thenReturn(Optional.empty());

        Optional<ChatMessage> result = chatMessageService.find(messageId);

        assertFalse(result.isPresent());
        verify(chatMessageRepository).findById(messageId);
    }

    @Test
    void testFindAllByChat() {
        PageRequest pageRequest = PageRequest.of(0, 10);
        Page<ChatMessage> messagePage = new PageImpl<>(List.of(chatMessage), pageRequest, 1);
        when(chatMessageRepository.findAllByChat(chat, pageRequest)).thenReturn(messagePage);

        Page<ChatMessage> result = chatMessageService.findAllByChat(chat, pageRequest);

        assertEquals(1, result.getTotalElements());
        assertEquals(messageId, result.getContent().get(0).getId());
        verify(chatMessageRepository).findAllByChat(chat, pageRequest);
    }

    @Test
    void testCreate() {
        ChatMessage newMessage = ChatMessage.builder()
                .content("New message")
                .chat(chat)
                .chatMember(chatMember)
                .build();

        when(chatMessageRepository.save(any(ChatMessage.class))).thenAnswer(invocation -> {
            ChatMessage saved = invocation.getArgument(0);
            saved.setId(messageId);
            saved.setDateOfCreation(LocalDateTime.now());
            return saved;
        });

        ChatMessage result = chatMessageService.create(newMessage);

        assertNotNull(result);
        assertNotNull(result.getDateOfCreation());
        assertNotNull(result.getViewedBy());
        assertEquals("New message", result.getContent());
        verify(chatMessageRepository).save(newMessage);
    }

    @Test
    void testCreate_WithNullViewedBy() {
        ChatMessage newMessage = ChatMessage.builder()
                .content("New message")
                .chat(chat)
                .chatMember(chatMember)
                .viewedBy(null)
                .build();

        when(chatMessageRepository.save(any(ChatMessage.class))).thenAnswer(invocation -> {
            ChatMessage saved = invocation.getArgument(0);
            saved.setId(messageId);
            saved.setDateOfCreation(LocalDateTime.now());
            saved.setViewedBy(new ArrayList<>());
            return saved;
        });

        ChatMessage result = chatMessageService.create(newMessage);

        assertNotNull(result.getViewedBy());
        assertTrue(result.getViewedBy().isEmpty());
        verify(chatMessageRepository).save(newMessage);
    }

    @Test
    void testCreate_WithNullFirstAndLastName() {
        ChatMessage newMessage = ChatMessage.builder()
                .content("New message")
                .chat(chat)
                .chatMember(chatMember)
                .firstAndLastName(null)
                .build();

        when(chatMessageRepository.save(any(ChatMessage.class))).thenAnswer(invocation -> {
            ChatMessage saved = invocation.getArgument(0);
            saved.setId(messageId);
            saved.setDateOfCreation(LocalDateTime.now());
            saved.setViewedBy(new ArrayList<>());
            saved.setFirstAndLastName("Test User");
            return saved;
        });

        ChatMessage result = chatMessageService.create(newMessage);

        assertEquals("Test User", result.getFirstAndLastName());
        verify(chatMessageRepository).save(newMessage);
    }

    @Test
    void testCreate_WithExistingFirstAndLastName() {
        ChatMessage newMessage = ChatMessage.builder()
                .content("New message")
                .chat(chat)
                .chatMember(chatMember)
                .firstAndLastName("Existing Name")
                .build();

        when(chatMessageRepository.save(any(ChatMessage.class))).thenAnswer(invocation -> {
            ChatMessage saved = invocation.getArgument(0);
            saved.setId(messageId);
            saved.setDateOfCreation(LocalDateTime.now());
            saved.setViewedBy(new ArrayList<>());
            return saved;
        });

        ChatMessage result = chatMessageService.create(newMessage);

        assertEquals("Existing Name", result.getFirstAndLastName());
        verify(chatMessageRepository).save(newMessage);
    }

    @Test
    void testCreate_WithNullChatMember() {
        ChatMessage newMessage = ChatMessage.builder()
                .content("New message")
                .chat(chat)
                .chatMember(null)
                .firstAndLastName("Test Name")
                .build();

        when(chatMessageRepository.save(any(ChatMessage.class))).thenAnswer(invocation -> {
            ChatMessage saved = invocation.getArgument(0);
            saved.setId(messageId);
            saved.setDateOfCreation(LocalDateTime.now());
            saved.setViewedBy(new ArrayList<>());
            return saved;
        });

        ChatMessage result = chatMessageService.create(newMessage);

        assertEquals("Test Name", result.getFirstAndLastName());
        verify(chatMessageRepository).save(newMessage);
    }

    @Test
    void testUpdate() {
        when(chatMessageRepository.save(chatMessage)).thenReturn(chatMessage);

        chatMessageService.update(chatMessage);

        verify(chatMessageRepository).save(chatMessage);
    }
}

