package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.util.List;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "GetChatMembersResponse DTO")
public class GetChatMembersResponse {
    @Getter
    @Setter
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor(access = AccessLevel.PRIVATE)
    @ToString
    @EqualsAndHashCode
    public static class ChatMember {
        @Schema(description = "Chat member id value")
        private UUID id;

        @Schema(description = "Chat member nickname value")
        private String nickname;

        @Schema(description = "Member id value")
        private UUID memberId;
    }

    @Singular
    @Schema(description = "Chat member list")
    private List<ChatMember> chatMembers;

    @Schema(description = "Number of all objects")
    private Integer count;
}
