package com.keepitup.calendar.api.Calendar.API.timeentry.dto;

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
@Schema(description = "GetTimeEntryResponse DTO")
public class GetTimeEntrysResponse {

    @Getter
    @Setter
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor(access = AccessLevel.PRIVATE)
    @ToString
    @EqualsAndHashCode
    public static class TimeEntry {
        @Schema(description = "TimeEntry id value")
        private UUID id;
    }

    @Singular("timeEntry")
    @Schema(description = "TimeEntry list")
    private List<TimeEntry> timeEntryList;

    @Schema(description = "Number of all objects")
    private Integer count;
}