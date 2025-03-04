package com.keepitup.calendar.api.Calendar.API.timeentry.function;

import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto.GetTimeEntryTemplateResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class TimeEntryToResponseFunction implements Function<TimeEntryTemplate, GetTimeEntryTemplateResponse> {

    @Override
    public GetTimeEntryTemplateResponse apply(TimeEntryTemplate timeEntryTemplate) {
        return GetTimeEntryTemplateResponse.builder()
                .startTime(timeEntryTemplate.getStartTime())
                .endTime(timeEntryTemplate.getEndTime())
                .startDayOffset(timeEntryTemplate.getStartDayOffset())
                .endDayOffset(timeEntryTemplate.getEndDayOffset())
                .build();
    }
}
