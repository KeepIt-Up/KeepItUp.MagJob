package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function;

import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.GetAvailabilityTemplateResponse;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class AvailabilityTemplateToResponseFunction implements Function<AvailabilityTemplate, GetAvailabilityTemplateResponse> {

    @Override
    public GetAvailabilityTemplateResponse apply(AvailabilityTemplate timeEntryTemplate) {
        return GetAvailabilityTemplateResponse.builder()
                .id(timeEntryTemplate.getId())
                .name(timeEntryTemplate.getName())
                .numberOfDays(timeEntryTemplate.getNumberOfDays())
                .organizationId(timeEntryTemplate.getOrganizationId())
                .startDayOfWeek(timeEntryTemplate.getStartDayOfWeek())
                .timeEntryTemplates(timeEntryTemplate.getTimeEntryTemplates())
                .build();
    }
}
