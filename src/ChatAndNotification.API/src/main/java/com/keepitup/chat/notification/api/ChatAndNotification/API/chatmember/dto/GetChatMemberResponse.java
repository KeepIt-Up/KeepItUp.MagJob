package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.dto;

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
@Schema(description = "GetChatMemberResponse DTO")
public class GetChatMemberResponse {
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

        @Schema(description = "Chat title")
        private String title;
    }

    @Schema(description = "Chat member id value")
    private UUID id;

    @Schema(description = "Member id value")
    private UUID memberId;

    @Schema(description = "Chat class value")
    private Chat chat;

    @Schema(description = "Chat member nickname value")
    private String nickname;

    @Schema(description = "Chat member is invitation accepted value")
    private Boolean isInvitationAccepted;
}
