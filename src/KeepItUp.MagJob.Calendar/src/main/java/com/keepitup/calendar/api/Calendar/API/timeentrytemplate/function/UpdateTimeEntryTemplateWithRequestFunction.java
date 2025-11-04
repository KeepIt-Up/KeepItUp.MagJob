package com.keepitup.calendar.api.Calendar.API.timeentrytemplate.function;

import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto.*;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class UpdateTimeEntryTemplateWithRequestFunction implements BiFunction<TimeEntryTemplate, PatchTimeEntryTemplateRequest, TimeEntryTemplate> {
    @Override
    public TimeEntryTemplate apply(TimeEntryTemplate timeEntryTemplate, PatchTimeEntryTemplateRequest request) {
        return TimeEntryTemplate.builder()
                .id(timeEntryTemplate.getId())
                .build();
    }
}
