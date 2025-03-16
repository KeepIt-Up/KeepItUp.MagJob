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
@Schema(description = "PostChatMessageRequest DTO")
public class PostChatMessageRequest {
    @Schema(description = "ChatMessage content")
    private String content;

    @Schema(description = "ChatMessage attachment")
    private byte[] attachment;

    @Schema(description = "ChatMessage chatMember id value")
    private UUID chatMember;

    @Schema(description = "ChatMessage chat id value")
    private UUID chat;
}
