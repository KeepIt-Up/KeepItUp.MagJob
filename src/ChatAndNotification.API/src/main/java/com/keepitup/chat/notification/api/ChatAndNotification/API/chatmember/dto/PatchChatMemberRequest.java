package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "PatchChatMemberRequest DTO")
public class PatchChatMemberRequest {
    @Schema(description = "Chat Member nickname value")
    private String nickname;
}
