package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.respository.api;

import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.entity.Notification;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.UUID;

public interface NotificationRepository extends JpaRepository<Notification, UUID> {
    Page<Notification> findAllBySeen(boolean seen, Pageable pageable);
    Page<Notification> findAllBySent(boolean sent, Pageable pageable);
    Page<Notification> findAllByUserId(UUID userId, Pageable pageable);
    Page<Notification> findAllByUserIdAndSeen(UUID userId, Boolean seen, Pageable pageable);
    Page<Notification> findAllByOrganizationId(UUID organizationId, Pageable pageable);
    Page<Notification> findAllByOrganizationIdAndSeen(UUID organizationId, Boolean seen, Pageable pageable);
    Page<Notification> findAllByMemberId(UUID memberId, Pageable pageable);
    Page<Notification> findAllByMemberIdAndSeen(UUID memberId, Boolean seen, Pageable pageable);

}
