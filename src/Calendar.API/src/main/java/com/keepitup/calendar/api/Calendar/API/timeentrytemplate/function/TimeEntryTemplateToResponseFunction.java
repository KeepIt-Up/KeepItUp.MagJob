package com.keepitup.calendar.api.Calendar.API.timeentrytemplate.function;

import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto.GetTimeEntryTemplateResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class TimeEntryTemplateToResponseFunction implements Function<TimeEntryTemplate, GetTimeEntryTemplateResponse> {

    @Override
    public GetTimeEntryTemplateResponse apply(TimeEntryTemplate timeEntryTemplate) {
        return GetTimeEntryTemplateResponse.builder()
                .id(timeEntryTemplate.getId())
                .build();
    }
}
