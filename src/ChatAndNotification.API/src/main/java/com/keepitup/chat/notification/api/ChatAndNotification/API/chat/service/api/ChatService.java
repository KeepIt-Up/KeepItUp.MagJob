package com.keepitup.chat.notification.api.ChatAndNotification.API.chat.service.api;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.math.BigInteger;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface ChatService {
    Optional<Chat> find(UUID id);

    Optional<Chat> findByTitle(String title);

    List<Chat> findAll();

    Page<Chat> findAll(Pageable pageable);

    Page<Chat> findAllByOrganizationId(UUID organizationId, Pageable pageable);

    void addAdmin(Chat chat, ChatMember chatMember);

    void removeAdmin(Chat chat, ChatMember chatMember);

    void create(Chat chat);

    void delete(UUID id);

    void update(Chat chat);
}
