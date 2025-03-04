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
@Schema(description = "GetTimeEntryResponse DTO")
public class GetAvailabilityTemplateResponse {
    @Schema(description = "id")
    private UUID id;

    @Schema(description = "GetAvailibityTemplateRequest name value")
    private String name;

    @Schema(description = "GetAvailibityTemplateRequest organizationId value")
    private BigInteger organizationId;

    @Schema(description = "GetAvailibityTemplateRequest startDayOfWeek value")
    private String startDayOfWeek;

    @Schema(description = "GetAvailibityTemplateRequest numberOfDays value")
    private Integer numberOfDays;

    @Schema(description = "GetAvailibityTemplateRequest timeEntryTemplates value")
    private List<TimeEntryTemplate> timeEntryTemplates;
}