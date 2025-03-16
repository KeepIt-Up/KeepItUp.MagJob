package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto.GetNotificationsResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.entity.Notification;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class NotificationsToResponseFunction implements BiFunction<Page<Notification>, Integer, GetNotificationsResponse> {
    public GetNotificationsResponse apply(Page<Notification> notifications, Integer count) {
        return GetNotificationsResponse.builder()
                .notifications(notifications.stream()
                        .map(notification -> GetNotificationsResponse.Notification.builder()
                                .id(notification.getId())
                                .content(notification.getContent())
                                .seen(notification.isSeen())
                                .sent(notification.isSent())
                                .organizationId(notification.getOrganizationId() != null ? notification.getOrganizationId() : null)
                                .memberId(notification.getMemberId() != null ? notification.getMemberId() : null)
                                .userId(notification.getUserId() != null ? notification.getUserId() : null)
                                .build())
                        .toList())
                .count(count)
                .build();
    }
}
