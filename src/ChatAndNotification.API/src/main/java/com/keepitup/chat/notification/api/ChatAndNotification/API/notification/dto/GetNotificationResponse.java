package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.time.LocalDateTime;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "GetNotificationResponse DTO")
public class GetNotificationResponse {
    @Schema(description = "Notification id value")
    private UUID id;

    @Schema(description = "Notification content")
    private String content;

    @Schema(description = "Notification date of creation")
    private LocalDateTime dateOfCreation;

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
