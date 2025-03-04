package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function;

import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.PostAvailabilityTemplateRequest;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class RequestToAvailabilityTemplateFunction implements Function<PostAvailabilityTemplateRequest, AvailabilityTemplate> {
    @Override
    public AvailabilityTemplate apply(PostAvailabilityTemplateRequest request) {
        return AvailabilityTemplate.builder()
                .id(request.getId())
                .name(request.getName())
                .startDayOfWeek(request.getStartDayOfWeek())
                .organizationId(request.getOrganizationId())
                .numberOfDays(request.getNumberOfDays())
                .timeEntryTemplates(request.getTimeEntryTemplates())
                .build();
    }
}
