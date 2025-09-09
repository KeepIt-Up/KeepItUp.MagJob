package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto;

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
@Schema(description = "TypingEventRequest DTO")
public class TypingEventRequest {
    @Schema(description = "Typing event type")
    private String type; // "TYPING_START" or "TYPING_STOP"

    @Schema(description = "Chat id value")
    private UUID chatId;

    @Schema(description = "Member id value")
    private UUID memberId;

    @Schema(description = "Member name")
    private String memberName;

    @Schema(description = "Timestamp of the event")
    private String timestamp;
}
