package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.repository.api;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;
import java.util.UUID;

@Repository
public interface ChatMemberRepository extends JpaRepository<ChatMember, UUID> {
    Page<ChatMember> findAllByMemberId(UUID memberId, Pageable pageable);
    Optional<ChatMember> findByMemberIdAndChat(UUID memberId, Chat chat);
    Page<ChatMember> findAllByChatAndIsInvitationAccepted(Chat chat, Boolean isInvitationAccepted, Pageable pageable);
}
