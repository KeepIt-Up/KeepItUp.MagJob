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
@Schema(description = "GetChatMessageResponse DTO")
public class GetChatMessageResponse {
    @Schema(description = "ChatMessage id value")
    private UUID id;

    @Schema(description = "Chat content")
    private String content;

    @Schema(description = "ChatMessage date of creation")
    private LocalDate dateOfCreation;

    @Schema(description = "ChatMessage viewedBy")
    private List<String> viewedBy;

    @Schema(description = "ChatMessage firstAndLastName")
    private String firstAndLastName;

    @Schema(description = "ChatMessage attachment")
    private byte[] attachment;

    @Getter
    @Setter
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor(access = AccessLevel.PRIVATE)
    @ToString
    @EqualsAndHashCode
    public static class ChatMember {
        @Schema(description = "Chat Member id value")
        private UUID id;

        @Schema(description = "Chat Member nickname name")
        private String nickname;

        @Schema(description = "Member id value")
        private UUID memberId;
    }

    @Getter
    @Setter
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor(access = AccessLevel.PRIVATE)
    @ToString
    @EqualsAndHashCode
    public static class Chat {
        @Schema(description = "Chat id value")
        private UUID id;

        @Schema(description = "Chat title name")
        private String title;

        @Schema(description = "Organization id value")
        private UUID organizationId;
    }

    @Schema(description = "Chat Member class value")
    private ChatMember chatMember;

    @Schema(description = "Chat class value")
    private Chat chat;
}
