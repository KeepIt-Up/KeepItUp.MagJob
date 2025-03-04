package com.keepitup.calendar.api.Calendar.API.timeentrymember.dto;

import com.keepitup.calendar.api.Calendar.API.member.entity.Member;
import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

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
public class PostTimeEntryMemberRequest {
    @Schema(description = "id")
    private UUID id;

    @Schema(description = "PostTimeEntryMemberRequest status value")
    private String status;

    @Schema(description = "PostTimeEntryMemberRequest member value")
    private Member member;

    @Schema(description = "PostTimeEntryMemberRequest timeEntry value")
    private TimeEntry timeEntry;
}