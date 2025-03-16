package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.service.impl;

import com.keepitup.chat.notification.api.ChatAndNotification.API.configuration.Constants;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.entity.Notification;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.respository.api.NotificationRepository;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.service.api.NotificationService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ResponseStatusException;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class NotificationDefaultService implements NotificationService {
    private final NotificationRepository notificationRepository;
    private final SimpMessagingTemplate messagingTemplate;

    @Autowired
    public NotificationDefaultService(NotificationRepository notificationRepository, SimpMessagingTemplate messagingTemplate) {
        this.notificationRepository = notificationRepository;
        this.messagingTemplate = messagingTemplate;
    }

    @Override
    public Optional<Notification> find(UUID id) {
        return notificationRepository.findById(id);
    }

    @Override
    public List<Notification> findAll() {
        return notificationRepository.findAll();
    }

    @Override
    public Page<Notification> findAll(Pageable pageable) {
        return notificationRepository.findAll(pageable);
    }

    @Override
    public Page<Notification> findAllBySeen(boolean seen, Pageable pageable) {
        return notificationRepository.findAllBySeen(seen, pageable);
    }

    @Override
    public Page<Notification> findAllBySent(boolean sent, Pageable pageable) {
        return notificationRepository.findAllBySent(sent, pageable);
    }

    @Override
    public Page<Notification> findAllByOrganizationId(UUID organizationId, Pageable pageable) {
        return notificationRepository.findAllByOrganizationId(organizationId, pageable);
    }

    @Override
    public Page<Notification> findAllByOrganizationIdAndSeen(UUID organizationId, Boolean seen, Pageable pageable) {
        return notificationRepository.findAllByOrganizationIdAndSeen(organizationId, seen, pageable);
    }

    @Override
    public Page<Notification> findAllByMemberId(UUID memberId, Pageable pageable) {
        return notificationRepository.findAllByMemberId(memberId, pageable);
    }

    @Override
    public Page<Notification> findAllByMemberIdAndSeen(UUID memberId, Boolean seen, Pageable pageable) {
        return notificationRepository.findAllByMemberIdAndSeen(memberId, seen, pageable);
    }

    @Override
    public Page<Notification> findAllByUserId(UUID userId, Pageable pageable) {
        return notificationRepository.findAllByUserId(userId, pageable);
    }

    @Override
    public Page<Notification> findAllByUserIdAndSeen(UUID userId, Boolean seen, Pageable pageable) {
        return notificationRepository.findAllByUserIdAndSeen(userId, seen, pageable);
    }

    @Override
    public Notification create(Notification notification) {
        notification.setDateOfCreation(LocalDateTime.now());
        notification.setSeen(false);
        notification.setSent(false);

        sendNotificationToWebSocket(notification);

        return notificationRepository.save(notification);
    }

    @Override
    public void sendNotificationToWebSocket(Notification notification) {
        String destination;

        if (notification.getUserId() != null) {
            destination = String.join(
                    "",
                    Constants.NOTIFICATION_USER_DEFAULT_WEBSOCKET_ENDPOINT,
                    notification.getUserId().toString(),
                    Constants.NOTIFICATION_ENDPOINT
            );
        } else if (notification.getMemberId() != null) {
            destination = String.join(
                    "",
                    Constants.NOTIFICATION_MEMBER_DEFAULT_WEBSOCKET_ENDPOINT,
                    notification.getMemberId().toString(),
                    Constants.NOTIFICATION_ENDPOINT
            );
        } else if (notification.getOrganizationId() != null) {
            destination = String.join(
                    "",
                    Constants.NOTIFICATION_ORGANIZATION_DEFAULT_WEBSOCKET_ENDPOINT,
                    notification.getOrganizationId().toString(),
                    Constants.NOTIFICATION_ENDPOINT
            );
        } else {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }

        messagingTemplate.convertAndSend(destination, notification);
    }

    @Override
    public void update(Notification notification) {
        notificationRepository.save(notification);
    }

    @Override
    public void delete(UUID id) {
        notificationRepository.findById(id).ifPresent(notificationRepository::delete);
    }
}
