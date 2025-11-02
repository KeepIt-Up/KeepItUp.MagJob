package com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.math.BigInteger;
import java.time.LocalTime;
import java.time.ZonedDateTime;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "GetTimeEntryResponse DTO")
public class GetTimeEntryTemplateResponse {
    @Schema(description = "PostTimeEntryTemplateRequest startTime value")
    private LocalTime startTime;

    @Schema(description = "PostTimeEntryTemplateRequest endTime value")
    private LocalTime endTime;

    @Schema(description = "PostTimeEntryTemplateRequest startDayOffset value")
    private Integer startDayOffset;

    @Schema(description = "PostTimeEntryTemplateRequest endDayOffset value")
    private Integer endDayOffset;
}