package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.math.BigInteger;
import java.util.List;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "GetNotificationsResponse DTO")
public class GetNotificationsResponse {
    @Getter
    @Setter
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor(access = AccessLevel.PRIVATE)
    @ToString
    @EqualsAndHashCode
    public static class Notification {
        @Schema(description = "Notification id value")
        private UUID id;

        @Schema(description = "Notification content")
        private String content;

        @Schema(description = "Notification seen value")
        private boolean seen;

        @Schema(description = "Notification sent value")
        private boolean sent;

        @Schema(description = "Organization id value")
        private UUID organizationId;

        @Schema(description = "Member id value")
        private UUID memberId;

        @Schema(description = "User id value")
        private UUID userId;
    }

    @Singular
    @Schema(description = "Chat list")
    private List<Notification> notifications;

    @Schema(description = "Number of all objects")
    private Integer count;
}
