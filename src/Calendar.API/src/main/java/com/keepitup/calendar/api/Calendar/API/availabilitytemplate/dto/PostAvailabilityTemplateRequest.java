package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto;


import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.persistence.Column;
import jakarta.validation.constraints.NotNull;
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
public class PostAvailabilityTemplateRequest {
    @Schema(description = "id")
    private UUID id;

    @Schema(description = "PostAvailabilityTemplateRequest name value")
    private String name;

    @Schema(description = "PostAvailabilityTemplateRequest organizationId value")
    private BigInteger organizationId;

    @Schema(description = "PostAvailabilityTemplateRequest startDayOfWeek value")
    private String startDayOfWeek;

    @Schema(description = "PostAvailabilityTemplateRequest numberOfDays value")
    private Integer numberOfDays;

    @Schema(description = "PostAvailabilityTemplateRequest timeEntryTemplates value")
    private List<TimeEntryTemplate> timeEntryTemplates;
}
