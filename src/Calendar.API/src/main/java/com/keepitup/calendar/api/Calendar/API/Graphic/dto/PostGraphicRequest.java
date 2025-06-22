package com.keepitup.calendar.api.Calendar.API.Graphic.dto;


import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
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
@Schema(description = "GetTimeEntryResponses DTO")
public class PostGraphicRequest {
    @Schema(description = "GetGraphicRequest name value")
    private String name;

    @Schema(description = "GetGraphicRequest managerId value")
    private BigInteger managerId;

    @Schema(description = "GetGraphicRequest TimeEntryMembers value")
    private List<TimeEntryMember> timeEntryMembers;

    @Schema(description = "GetGraphicRequest TimeEntries value")
    private List<TimeEntry> timeEntries;
}
