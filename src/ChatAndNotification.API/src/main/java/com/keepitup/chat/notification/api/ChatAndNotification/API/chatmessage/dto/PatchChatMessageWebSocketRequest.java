package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.math.BigInteger;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "PatchChatMessageWebSocketRequest DTO")
public class PatchChatMessageWebSocketRequest {
    @Schema(description = "chat message id value")
    private UUID chatMessageId;

    @Schema(description = "chat message viewed by value")
    private String viewedBy;
}
