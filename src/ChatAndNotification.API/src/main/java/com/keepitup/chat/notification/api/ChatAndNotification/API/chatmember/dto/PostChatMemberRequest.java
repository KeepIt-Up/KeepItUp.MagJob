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
@Schema(description = "PostChatMemberRequest DTO")
public class PostChatMemberRequest {
    @Schema(description = "Chat Member nickname value")
    private String nickname;

    @Schema(description = "Member id value")
    private UUID memberId;

    @Schema(description = "Chat id value")
    private UUID chatId;
}
