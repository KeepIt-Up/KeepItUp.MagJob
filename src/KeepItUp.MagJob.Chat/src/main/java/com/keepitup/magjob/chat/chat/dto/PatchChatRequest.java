package com.keepitup.magjob.chat.chat.dto;

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
public class PatchChatRequest {
    @Schema(description = "Chat title")
    private String title;
}
