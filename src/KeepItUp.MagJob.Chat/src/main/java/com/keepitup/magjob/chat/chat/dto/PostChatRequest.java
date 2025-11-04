package com.keepitup.magjob.chat.chat.dto;

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
@Schema(description = "PostChatRequest DTO")
public class PostChatRequest {
    @Schema(description = "Chat title")
    private String title;

    @Schema(description = "Organization id value")
    private UUID organizationId;

    @Schema(description = "Member id value")
    private UUID memberId;

    @Schema(description = "Member nickname")
    private String nickname;
}
