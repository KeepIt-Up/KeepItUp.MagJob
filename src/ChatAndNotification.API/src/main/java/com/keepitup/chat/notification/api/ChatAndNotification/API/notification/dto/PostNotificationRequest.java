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
@Schema(description = "PostNotificationRequest DTO")
public class PostNotificationRequest {
    @Schema(description = "Notification content")
    private String content;

    @Schema(description = "Organization id value")
    private UUID organizationId;

    @Schema(description = "Member id value")
    private UUID memberId;

    @Schema(description = "User id value")
    private UUID userId;
}
