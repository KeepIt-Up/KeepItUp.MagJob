package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto.PatchNotificationRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.entity.Notification;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class UpdateNotificationWithRequestFunction implements BiFunction<Notification, PatchNotificationRequest, Notification> {
    @Override
    public Notification apply(Notification notification, PatchNotificationRequest patchNotificationRequest) {
        return Notification.builder()
                .id(notification.getId())
                .content(notification.getContent())
                .seen(patchNotificationRequest.isSeen())
                .sent(patchNotificationRequest.isSent())
                .organizationId(notification.getOrganizationId())
                .memberId(notification.getMemberId())
                .userId(notification.getUserId())
                .dateOfCreation(notification.getDateOfCreation())
                .build();
    }
}
