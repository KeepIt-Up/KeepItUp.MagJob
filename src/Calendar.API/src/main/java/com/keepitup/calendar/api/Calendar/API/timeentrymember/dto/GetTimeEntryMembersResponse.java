package com.keepitup.calendar.api.Calendar.API.timeentrymember.dto;

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
    }

    @Singular("timeEntryMember")
    @Schema(description = "TimeEntryMember list")
    private List<TimeEntryMember> timeEntryMemberList;

    @Schema(description = "Number of all objects")
    private Integer count;
}