package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.controller.impl;

//import com.keepitup.chat.notification.api.ChatAndNotification.API.configuration.SecurityService;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.controller.api.NotificationController;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto.GetNotificationResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto.GetNotificationsResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto.PatchNotificationRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto.PostNotificationRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.entity.Notification;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.function.NotificationToResponseFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.function.NotificationsToResponseFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.function.RequestToNotificationFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.function.UpdateNotificationWithRequestFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.service.impl.NotificationDefaultService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Controller;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigInteger;
import java.util.UUID;

@Controller
@Log
public class NotificationDefaultController implements NotificationController {
    private final NotificationDefaultService notificationService;
    //private final SecurityService securityService;
    private final NotificationToResponseFunction notificationToResponseFunction;
    private final NotificationsToResponseFunction notificationsToResponseFunction;
    private final RequestToNotificationFunction requestToNotificationFunction;
    private final UpdateNotificationWithRequestFunction updateNotificationWithRequestFunction;

    @Autowired
    public NotificationDefaultController(
        NotificationDefaultService notificationService,
        //SecurityService securityService,
        NotificationToResponseFunction notificationToResponseFunction,
        NotificationsToResponseFunction notificationsToResponseFunction,
        RequestToNotificationFunction requestToNotificationFunction,
        UpdateNotificationWithRequestFunction updateNotificationWithRequestFunction
    ) {
        this.notificationService = notificationService;
        //this.securityService = securityService;
        this.notificationToResponseFunction = notificationToResponseFunction;
        this.notificationsToResponseFunction = notificationsToResponseFunction;
        this.requestToNotificationFunction = requestToNotificationFunction;
        this.updateNotificationWithRequestFunction = updateNotificationWithRequestFunction;
    }

    @Override
    public GetNotificationsResponse getNotifications(int page, int size) {
//        if (!securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PageRequest pageRequest = PageRequest.of(page, size);
        Integer count = notificationService.findAll().size();
        return notificationsToResponseFunction.apply(notificationService.findAll(pageRequest), count);
    }

    @Override
    public GetNotificationsResponse getSeenOrUnseenNotifications(int page, int size, boolean seen) {
//        if (!securityService.hasAdminPermission()) {
//            System.out.println(securityService.hasAdminPermission());
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PageRequest pageRequest = PageRequest.of(page, size);
        Integer count = notificationService.findAllBySeen(seen, Pageable.unpaged()).getNumberOfElements();
        return notificationsToResponseFunction.apply(notificationService.findAllBySeen(seen, pageRequest), count);
    }

    @Override
    public GetNotificationsResponse getSentOrNotSentNotifications(int page, int size, boolean sent) {
//        if (!securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PageRequest pageRequest = PageRequest.of(page, size);
        Integer count = notificationService.findAllBySent(sent, Pageable.unpaged()).getNumberOfElements();
        return notificationsToResponseFunction.apply(notificationService.findAllBySent(sent, pageRequest), count);
    }

    @Override
    public GetNotificationResponse getNotification(UUID id) {
//        if (!securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        Notification notification = notificationService.find(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        return notificationToResponseFunction.apply(notification);
    }

    @Override
    public GetNotificationsResponse getNotificationsByOrganization(int page, int size, UUID organizationId) {
//        Organization organization = organizationService.find(organizationId).orElseThrow(
//                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
//        );
//
//        if (!securityService.belongsToOrganization(organization) && !securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Integer count = notificationService.findAllByOrganizationId(organizationId, Pageable.unpaged()).getNumberOfElements();

        return notificationsToResponseFunction.apply(notificationService.findAllByOrganizationId(organizationId, pageRequest), count);
    }

    @Override
    public GetNotificationsResponse getSeenOrUnseenNotificationsByOrganization(int page, int size, UUID organizationId, boolean seen) {
//        Organization organization = organizationService.find(organizationId).orElseThrow(
//                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
//        );
//
//        if (!securityService.belongsToOrganization(organization) && !securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Integer count = notificationService.findAllByOrganizationIdAndSeen(organizationId, seen, Pageable.unpaged()).getNumberOfElements();

        return notificationsToResponseFunction.apply(notificationService.findAllByOrganizationIdAndSeen(organizationId, seen, pageRequest), count);
    }

    @Override
    public GetNotificationsResponse getNotificationsByMember(int page, int size, UUID memberId) {
//        Member member = memberService.find(memberId).orElseThrow(
//                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
//        );
//
//        if (!securityService.isCurrentMember(member) && !securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Integer count = notificationService.findAllByMemberId(memberId, Pageable.unpaged()).getNumberOfElements();

        return notificationsToResponseFunction.apply(notificationService.findAllByMemberId(memberId, pageRequest), count);
    }

    @Override
    public GetNotificationsResponse getSeenOrUnseenNotificationsByMember(int page, int size, UUID memberId, boolean seen) {
//        Member member = memberService.find(memberId).orElseThrow(
//                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
//        );
//
//        if (!securityService.isCurrentMember(member) && !securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Integer count = notificationService.findAllByMemberIdAndSeen(memberId, seen, Pageable.unpaged()).getNumberOfElements();

        return notificationsToResponseFunction.apply(notificationService.findAllByMemberIdAndSeen(memberId, seen, pageRequest), count);
    }

    @Override
    public GetNotificationsResponse getNotificationsByUser(int page, int size, UUID userId) {
//        User user = userService.find(userId).orElseThrow(
//                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
//        );
//
//        if (!securityService.isCurrentUser(user) && !securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Integer count = notificationService.findAllByUserId(userId, Pageable.unpaged()).getNumberOfElements();

        return notificationsToResponseFunction.apply(notificationService.findAllByUserId(userId, pageRequest), count);
    }

    @Override
    public GetNotificationsResponse getSeenOrUnseenNotificationsByUser(int page, int size, UUID userId, boolean seen) {
//        User user = userService.find(userId).orElseThrow(
//                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
//        );
//
//        if (!securityService.isCurrentUser(user) && !securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Integer count = notificationService.findAllByUserIdAndSeen(userId, seen, Pageable.unpaged()).getNumberOfElements();

        return notificationsToResponseFunction.apply(notificationService.findAllByUserIdAndSeen(userId, seen, pageRequest), count);
    }

    @Override
    public GetNotificationResponse createNotification(PostNotificationRequest postNotificationRequest) {
        boolean hasOrganization = postNotificationRequest.getOrganizationId() != null;
        boolean hasMember = postNotificationRequest.getOrganizationId() != null;
        boolean hasUser = postNotificationRequest.getOrganizationId() != null;

        int targetCount = (hasOrganization ? 1 : 0) + (hasMember ? 1 : 0) + (hasUser ? 1 : 0);
        if (targetCount > 1) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST);
        }

        if (hasUser) {
//            User user = userService.find(postNotificationRequest.getUser()).orElseThrow(
//                    () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
//            );
//
//            if (!securityService.hasAdminPermission() && !securityService.isCurrentUser(user)
//                && !securityService.isAnyMember()) {
//                throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//            }
        } else if (hasOrganization) {
//            Organization organization = organizationService.find(postNotificationRequest.getOrganization()).orElseThrow(
//                    () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
//            );
//
//            if (!securityService.hasAdminPermission() && !securityService.belongsToOrganization(organization)) {
//                throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//            }
        } else if (hasMember) {
//            Member member = memberService.find(postNotificationRequest.getMember()).orElseThrow(
//                    () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
//            );
//
//            if (!securityService.hasAdminPermission() && !securityService.belongsToOrganization(member.getOrganization())) {
//                throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//            }
        }

        Notification notification = notificationService.create(requestToNotificationFunction.apply(postNotificationRequest));

        if (notification == null) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }

        return notificationToResponseFunction.apply(notification);
    }

    @Override
    public GetNotificationResponse updateNotificationAsSeen(UUID id) {
//        if (!securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        Notification notification = notificationService.find(id).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        PatchNotificationRequest patchNotificationRequest = new PatchNotificationRequest();
        patchNotificationRequest.setSeen(true);
        patchNotificationRequest.setSent(notification.isSent());

        notificationService.update(updateNotificationWithRequestFunction.apply(notification, patchNotificationRequest));

        return notificationService.find(id)
                .map(notificationToResponseFunction)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public GetNotificationResponse updateNotificationAsSent(UUID id) {
//        if (!securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        Notification notification = notificationService.find(id).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        PatchNotificationRequest patchNotificationRequest = new PatchNotificationRequest();
        patchNotificationRequest.setSeen(notification.isSeen());
        patchNotificationRequest.setSent(true);

        notificationService.update(updateNotificationWithRequestFunction.apply(notification, patchNotificationRequest));

        return notificationService.find(id)
                .map(notificationToResponseFunction)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public void deleteNotification(UUID id) {
//        if (!securityService.hasAdminPermission()) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        notificationService.find(id).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        notificationService.delete(id);
    }
}
