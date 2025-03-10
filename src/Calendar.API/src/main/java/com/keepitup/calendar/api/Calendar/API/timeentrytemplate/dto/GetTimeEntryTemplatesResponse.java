package com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.math.BigInteger;
import java.util.List;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "GetTimeEntryTemplatesResponse DTO")
public class GetTimeEntryTemplatesResponse {
    @Getter
    @Setter
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor(access = AccessLevel.PRIVATE)
    @ToString
    @EqualsAndHashCode
    public static class TimeEntryTemplate {
        @Schema(description = "TimeEntryTemplate id value")
        private UUID id;
    }

    @Singular("timeEntryTemplate")
    @Schema(description = "TimeEntryTemplate list")
    private List<TimeEntryTemplate> timeEntryTemplateList;

    @Schema(description = "Number of all objects")
    private Integer count;
}