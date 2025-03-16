package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.service.api;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

public interface ChatMemberService {
    Page<ChatMember> findAllByMemberId(UUID memberId, Pageable pageable);

    Page<ChatMember> findAllAcceptedMembers(Chat chat, Pageable pageable);

    Page<ChatMember> findAllPendingInvitationMembers(Chat chat, Pageable pageable);

    Page<Chat> findAllChatsByMemberId(UUID memberId, Pageable pageable);

    Optional<ChatMember> find(UUID id);

    Optional<ChatMember> findByMemberIdAndChat(UUID memberId, Chat chat);

    void create(ChatMember chatMember);

    void acceptInvitation(ChatMember chatMember);

    void delete(UUID id);

    void update(ChatMember chatMember);
}
