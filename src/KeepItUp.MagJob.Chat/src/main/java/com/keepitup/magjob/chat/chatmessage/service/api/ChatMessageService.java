package com.keepitup.magjob.chat.chatmessage.service.api;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chatmessage.entity.ChatMessage;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.util.Optional;
import java.util.UUID;

public interface ChatMessageService {
    Optional<ChatMessage> find(UUID id);
    Page<ChatMessage> findAllByChat(Chat chat, Pageable pageable);
    ChatMessage create(ChatMessage chatMessage);
    void update(ChatMessage chatMessage);
}
