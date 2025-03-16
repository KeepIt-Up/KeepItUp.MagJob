package com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto;

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
@Schema(description = "GetChatsResponse DTO")
public class GetChatsResponse {
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

        @Schema(description = "Organization id value")
        private UUID organizationId;
    }

    @Singular
    @Schema(description = "Chat list")
    private List<Chat> chats;

    @Schema(description = "Number of all objects")
    private Integer count;
}
