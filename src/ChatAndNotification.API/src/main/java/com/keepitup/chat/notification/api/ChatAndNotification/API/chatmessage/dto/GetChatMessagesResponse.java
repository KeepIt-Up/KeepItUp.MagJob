package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.math.BigInteger;
import java.time.LocalDate;
import java.util.List;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "GetChatMessagesResponse DTO")
public class GetChatMessagesResponse {
    @Getter
    @Setter
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor(access = AccessLevel.PRIVATE)
    @ToString
    @EqualsAndHashCode
    public static class ChatMessage {
        @Schema(description = "ChatMessage id value")
        private UUID id;

        @Schema(description = "ChatMessage content")
        private String content;

        @Schema(description = "ChatMember id value")
        private UUID chatMemberId;

        @Schema(description = "Chat id value")
        private UUID chatId;

        @Schema(description = "ChatMessage viewedBy")
        private List<String> viewedBy;

        @Schema(description = "ChatMessage firstAndLastName")
        private String firstAndLastName;

        @Schema(description = "ChatMessage attachment")
        private byte[] attachment;

        @Schema(description = "ChatMessage date of creation")
        private LocalDate dateOfCreation;
    }

    @Singular
    @Schema(description = "ChatMessages list")
    private List<ChatMessage> chatMessages;

    @Schema(description = "Number of all objects")
    private Integer count;
}
