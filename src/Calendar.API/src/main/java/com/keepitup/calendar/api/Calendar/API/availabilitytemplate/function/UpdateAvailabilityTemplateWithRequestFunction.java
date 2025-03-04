package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function;

import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.PatchAvailabilityTemplateRequest;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class UpdateAvailabilityTemplateWithRequestFunction implements BiFunction<AvailabilityTemplate, PatchAvailabilityTemplateRequest, AvailabilityTemplate> {
    @Override
    public AvailabilityTemplate apply(AvailabilityTemplate availabilityTemplate, PatchAvailabilityTemplateRequest request) {
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
