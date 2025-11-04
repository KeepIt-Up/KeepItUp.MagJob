package com.keepitup.calendar.api.Calendar.API.timeentrymember.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "GetTimeEntryMembersResponse DTO")
public class GetTimeEntryMembersResponse {

    @Getter
    @Setter
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor(access = AccessLevel.PRIVATE)
    @ToString
    @EqualsAndHashCode
    public static class TimeEntryMember {
        @Schema(description = "TimeEntryMember id value")
        private UUID id;

        @Schema(description = "TimeEntryMember status value")
        private String status;

        @Schema(description = "TimeEntryMember memberId value")
        private UUID memberId;

        @Schema(description = "TimeEntryMember timeEntry value")
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

    @Singular("timeEntryMember")
    @Schema(description = "TimeEntryMember list")
    private List<TimeEntryMember> timeEntryMemberList;

    @Schema(description = "Number of all objects")
    private Integer count;
}