package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto.GetNotificationResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.entity.Notification;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class NotificationToResponseFunction implements Function<Notification, GetNotificationResponse> {
    @Override
    public GetNotificationResponse apply(Notification notification) {
        return GetNotificationResponse.builder()
                .id(notification.getId())
                .content(notification.getContent())
                .seen(notification.isSeen())
                .sent(notification.isSent())
                .organizationId(notification.getOrganizationId() != null ? notification.getOrganizationId() : null)
                .memberId(notification.getMemberId() != null ? notification.getMemberId() : null)
                .userId(notification.getUserId() != null ? notification.getUserId() : null)
                .build();
    }
}
