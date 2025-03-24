package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function;

import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.GetAvailabilityTemplateResponse;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto.GetTimeEntryTemplateResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class AvailabilityTemplateToResponseFunction implements Function<AvailabilityTemplate, GetAvailabilityTemplateResponse> {

    @Override
    public GetAvailabilityTemplateResponse apply(AvailabilityTemplate availabilityTemplate) {
        availabilityTemplate.getTimeEntryTemplates().forEach(timeEntryTemplate -> timeEntryTemplate.setAvailabilityTemplate(null));

        return GetAvailabilityTemplateResponse.builder()
                .id(availabilityTemplate.getId())
                .name(availabilityTemplate.getName())
                .numberOfDays(availabilityTemplate.getNumberOfDays())
                .organizationId(availabilityTemplate.getOrganizationId())
                .startDayOfWeek(availabilityTemplate.getStartDayOfWeek())
                .timeEntryTemplates(availabilityTemplate.getTimeEntryTemplates())
                .build();
    }
}
