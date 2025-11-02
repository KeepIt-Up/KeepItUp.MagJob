package com.keepitup.magjob.chat.chatmessage.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "PatchChatRequest DTO")
public class PatchChatMessageRequest {
    @Schema(description = "chat message viewed by value")
    private String viewedBy;
}
