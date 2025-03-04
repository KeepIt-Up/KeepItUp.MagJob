package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto;


import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.math.BigInteger;
import java.time.LocalTime;
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
public class PatchAvailabilityTemplateRequest {
    @Schema(description = "id")
    private UUID id;

    @Schema(description = "PatchAvailibityTemplateRequest name value")
    private String name;

    @Schema(description = "PatchAvailibityTemplateRequest organizationId value")
    private BigInteger organizationId;

    @Schema(description = "PatchAvailibityTemplateRequest startDayOfWeek value")
    private String startDayOfWeek;

    @Schema(description = "PatchAvailibityTemplateRequest numberOfDays value")
    private Integer numberOfDays;

    @Schema(description = "PatchAvailibityTemplateRequest timeEntryTemplates value")
    private List<TimeEntryTemplate> timeEntryTemplates;
}
