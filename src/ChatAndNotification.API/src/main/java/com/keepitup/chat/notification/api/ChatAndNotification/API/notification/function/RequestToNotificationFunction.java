package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto.PostNotificationRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.entity.Notification;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.entity.Notification.NotificationBuilder;
import org.springframework.cglib.core.internal.Function;
import org.springframework.stereotype.Component;

@Component
public class RequestToNotificationFunction implements Function<PostNotificationRequest, Notification> {
    @Override
    public Notification apply(PostNotificationRequest postNotificationRequest) {
        NotificationBuilder notificationBuilder = Notification.builder()
                .content(postNotificationRequest.getContent());

        notificationBuilder.organizationId(postNotificationRequest.getOrganizationId() != null ? postNotificationRequest.getOrganizationId() : null);
        notificationBuilder.memberId(postNotificationRequest.getMemberId() != null ? postNotificationRequest.getMemberId() : null);
        notificationBuilder.userId(postNotificationRequest.getUserId() != null ? postNotificationRequest.getUserId() : null);

        return notificationBuilder.build();
    }
}
