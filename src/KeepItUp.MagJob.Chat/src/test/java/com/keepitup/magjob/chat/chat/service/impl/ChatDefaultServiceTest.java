package com.keepitup.magjob.chat.chat.service.impl;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chat.repository.api.ChatRepository;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
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
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class ChatDefaultServiceTest {

    @Mock
    private ChatRepository chatRepository;

    @InjectMocks
    private ChatDefaultService chatService;

    private Chat chat;
    private ChatMember chatMember;
    private UUID chatId;
    private UUID organizationId;
    private UUID memberId;

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

        chatMember = ChatMember.builder()
                .id(UUID.randomUUID())
                .chat(chat)
                .memberId(memberId)
                .nickname("Test User")
                .chatMessages(new ArrayList<>())
                .build();
    }

    @Test
    void testFind() {
        when(chatRepository.findById(chatId)).thenReturn(Optional.of(chat));

        Optional<Chat> result = chatService.find(chatId);

        assertTrue(result.isPresent());
        assertEquals(chatId, result.get().getId());
        verify(chatRepository).findById(chatId);
    }

    @Test
    void testFind_NotFound() {
        when(chatRepository.findById(chatId)).thenReturn(Optional.empty());

        Optional<Chat> result = chatService.find(chatId);

        assertFalse(result.isPresent());
        verify(chatRepository).findById(chatId);
    }

    @Test
    void testFindByTitle() {
        String title = "Test Chat";
        when(chatRepository.findByTitle(title)).thenReturn(Optional.of(chat));

        Optional<Chat> result = chatService.findByTitle(title);

        assertTrue(result.isPresent());
        assertEquals(title, result.get().getTitle());
        verify(chatRepository).findByTitle(title);
    }

    @Test
    void testFindByTitle_NotFound() {
        String title = "Non-existent Chat";
        when(chatRepository.findByTitle(title)).thenReturn(Optional.empty());

        Optional<Chat> result = chatService.findByTitle(title);

        assertFalse(result.isPresent());
        verify(chatRepository).findByTitle(title);
    }

    @Test
    void testFindAll() {
        List<Chat> chats = List.of(chat);
        when(chatRepository.findAll()).thenReturn(chats);

        List<Chat> result = chatService.findAll();

        assertEquals(1, result.size());
        assertEquals(chatId, result.get(0).getId());
        verify(chatRepository).findAll();
    }

    @Test
    void testFindAll_Pageable() {
        PageRequest pageRequest = PageRequest.of(0, 10);
        Page<Chat> chatPage = new PageImpl<>(List.of(chat), pageRequest, 1);
        when(chatRepository.findAll(pageRequest)).thenReturn(chatPage);

        Page<Chat> result = chatService.findAll(pageRequest);

        assertEquals(1, result.getTotalElements());
        assertEquals(chatId, result.getContent().get(0).getId());
        verify(chatRepository).findAll(pageRequest);
    }

    @Test
    void testFindAllByOrganizationId() {
        PageRequest pageRequest = PageRequest.of(0, 10);
        Page<Chat> chatPage = new PageImpl<>(List.of(chat), pageRequest, 1);
        when(chatRepository.findAllByOrganizationId(organizationId, pageRequest)).thenReturn(chatPage);

        Page<Chat> result = chatService.findAllByOrganizationId(organizationId, pageRequest);

        assertEquals(1, result.getTotalElements());
        assertEquals(organizationId, result.getContent().get(0).getOrganizationId());
        verify(chatRepository).findAllByOrganizationId(organizationId, pageRequest);
    }

    @Test
    void testCreate() {
        Chat newChat = Chat.builder()
                .title("New Chat")
                .organizationId(organizationId)
                .build();

        when(chatRepository.save(any(Chat.class))).thenAnswer(invocation -> {
            Chat saved = invocation.getArgument(0);
            saved.setId(chatId);
            return saved;
        });

        chatService.create(newChat);

        assertNotNull(newChat.getDateOfCreation());
        assertEquals(LocalDate.now(), newChat.getDateOfCreation());
        verify(chatRepository).save(newChat);
    }

    @Test
    void testDelete() {
        when(chatRepository.findById(chatId)).thenReturn(Optional.of(chat));

        chatService.delete(chatId);

        verify(chatRepository).findById(chatId);
        verify(chatRepository).delete(chat);
    }

    @Test
    void testDelete_NotFound() {
        when(chatRepository.findById(chatId)).thenReturn(Optional.empty());

        chatService.delete(chatId);

        verify(chatRepository).findById(chatId);
        verify(chatRepository, never()).delete(any());
    }

    @Test
    void testUpdate() {
        when(chatRepository.save(chat)).thenReturn(chat);

        chatService.update(chat);

        verify(chatRepository).save(chat);
    }
}

