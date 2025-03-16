package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.service.api;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.entity.ChatMessage;
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
