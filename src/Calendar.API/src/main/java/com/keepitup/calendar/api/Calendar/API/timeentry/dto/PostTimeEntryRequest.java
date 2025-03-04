package com.keepitup.calendar.api.Calendar.API.timeentry.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "PostTimeEntryTemplateRequest DTO")
public class PostTimeEntryRequest {
    @Schema(description = "PostTimeEntryTemplateRequest Id value")
    private UUID id;

    @Schema(description = "PostTimeEntryTemplateRequest startDateTime value")
    private LocalDateTime startDateTime;

    @Schema(description = "PostTimeEntryTemplateRequest endDateTime value")
    private LocalDateTime endDateTime;
}