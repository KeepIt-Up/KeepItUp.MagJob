package com.keepitup.calendar.api.Calendar.API.timeentrymember.dto;

import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
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
@Schema(description = "Bulk TimeEntryMembers creation DTO")
public class PostTimeEntryMembersBulkRequest {
    @Schema(description = "TimeEntry ID")
    private UUID timeEntryId;
    
    @Schema(description = "List of member assignments")
    private List<MemberAssignment> memberAssignments;
    
    @Getter
    @Setter
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor(access = AccessLevel.PRIVATE)
    @ToString
    @EqualsAndHashCode
    @Schema(description = "Member assignment")
    public static class MemberAssignment {
        @Schema(description = "Member ID")
        private UUID memberId;
        
        @Schema(description = "Status")
        private String status;
    }
}
