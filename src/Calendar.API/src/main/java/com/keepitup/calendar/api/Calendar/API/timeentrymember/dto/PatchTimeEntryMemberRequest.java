package com.keepitup.calendar.api.Calendar.API.timeentrymember.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "PatchTimeEntryMemberRequest DTO")
public class PatchTimeEntryMemberRequest {
    @Schema(description = "TimeEntryMember status value")
    private String status;
}