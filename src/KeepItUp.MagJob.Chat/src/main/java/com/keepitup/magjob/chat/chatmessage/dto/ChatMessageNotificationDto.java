package com.keepitup.magjob.chat.chatmessage.dto;

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
@Schema(description = "ChatMessageNotificationDto DTO")
public class ChatMessageNotificationDto {
    @Schema(description = "Chat id value")
    private UUID chatId;

    @Schema(description = "Chat title")
    private String chatTitle;

    @Schema(description = "Organization id value")
    private UUID organizationId;

    @Schema(description = "Notification message")
    private String message;

    @Schema(description = "Sender name")
    private String senderName;
}

