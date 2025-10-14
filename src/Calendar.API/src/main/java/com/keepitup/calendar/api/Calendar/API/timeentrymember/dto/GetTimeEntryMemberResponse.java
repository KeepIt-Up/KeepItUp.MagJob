package com.keepitup.calendar.api.Calendar.API.timeentrymember.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.time.LocalDateTime;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "GetTimeEntryMemberResponse DTO")
public class GetTimeEntryMemberResponse {
    @Schema(description = "id")
    private UUID id;

    @Schema(description = "PostTimeEntryMemberRequest status value")
    private String status;

    @Schema(description = "PostTimeEntryMemberRequest memberId value")
    private UUID memberId;

    @Schema(description = "PostTimeEntryMemberRequest timeEntry value")
    private TimeEntry timeEntry;

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

        @Schema(description = "TimeEntry startDateTime value")
        private LocalDateTime startDateTime;

        @Schema(description = "TimeEntry endDateTime value")
        private LocalDateTime endDateTime;
    }
}