package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "PatchNotificationRequest DTO")
public class PatchNotificationRequest {
    @Schema(description = "Notification id value")
    private UUID id;

    @Schema(description = "Notification seen value")
    private boolean seen;

    @Schema(description = "Notification sent value")
    private boolean sent;
}
