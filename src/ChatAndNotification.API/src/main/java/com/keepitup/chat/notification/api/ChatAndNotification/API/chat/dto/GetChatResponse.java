package com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.time.LocalDate;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "GetChatResponse DTO")
public class GetChatResponse {
    @Schema(description = "Chat id value")
    private UUID id;

    @Schema(description = "Chat title")
    private String title;

    @Schema(description = "Chat date of creation")
    private LocalDate dateOfCreation;

    @Schema(description = "Chat organizationId")
    private UUID organizationId;
}
