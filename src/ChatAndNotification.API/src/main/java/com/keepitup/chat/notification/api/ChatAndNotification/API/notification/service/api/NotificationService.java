package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.service.api;

import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.entity.Notification;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;

import java.math.BigInteger;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface NotificationService {
    Optional<Notification> find(UUID id);
    List<Notification> findAll();
    Page<Notification> findAll(Pageable pageable);
    Page<Notification> findAllBySeen(boolean seen, Pageable pageable);
    Page<Notification> findAllBySent(boolean sent, Pageable pageable);
    Page<Notification> findAllByOrganizationId(UUID organizationId, Pageable pageable);
    Page<Notification> findAllByOrganizationIdAndSeen(UUID organizationId, Boolean seen, Pageable pageable);
    Page<Notification> findAllByMemberId(UUID memberId, Pageable pageable);
    Page<Notification> findAllByMemberIdAndSeen(UUID memberId, Boolean seen, Pageable pageable);
    Page<Notification> findAllByUserId(UUID userId, Pageable pageable);
    Page<Notification> findAllByUserIdAndSeen(UUID userId, Boolean seen, Pageable pageable);
    Notification create(Notification notification);
    void sendNotificationToWebSocket(Notification notification);
    void update(Notification notification);
    void delete(UUID id);


}
