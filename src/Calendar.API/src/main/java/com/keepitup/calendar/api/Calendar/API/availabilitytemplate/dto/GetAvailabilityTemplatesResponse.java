package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.util.List;


@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "GetTimeEntryResponses DTO")
public class GetAvailabilityTemplatesResponse {

    @Schema(description = "Availability Templates DTO")
    private List<GetAvailabilityTemplateResponse> availabilityTemplateResponseList;
}
