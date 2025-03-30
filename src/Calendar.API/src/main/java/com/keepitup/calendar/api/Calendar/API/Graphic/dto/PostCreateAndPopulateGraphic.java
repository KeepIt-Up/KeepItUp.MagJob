package com.keepitup.calendar.api.Calendar.API.Graphic.dto;

import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.time.LocalDate;
import java.util.List;
import java.util.UUID;
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
@Schema(description = "Graphic creation DTO")
public class PostCreateAndPopulateGraphic {
    @Schema(description = "availabilityTemplateId")
    private UUID availabilityTemplateId;

    @Schema(description = "PostCreateAndPopulateGraphic name value")
    private String name;

    @Schema(description = "PostCreateAndPopulateGraphic managerId value")
    private BigInteger managerId;

    @Schema(description = "PostCreateAndPopulateGraphic startDate value")
    private LocalDate startDate;

    @Schema(description = "PostCreateAndPopulateGraphic timeEntryTemplates value")
    private List<TimeEntryMember> timeEntryMembers;
}
