package com.keepitup.calendar.api.Calendar.API.timeentrytemplate.function;

import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto.*;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class TimeEntryTemplatesToResponseFunction implements BiFunction<Page<TimeEntryTemplate>, Integer, GetTimeEntryTemplatesResponse> {

    @Override
    public GetTimeEntryTemplatesResponse apply(Page<TimeEntryTemplate> entities, Integer count) {
        return GetTimeEntryTemplatesResponse.builder()
                .timeEntryTemplateList(entities.stream()
                        .map(organization -> GetTimeEntryTemplatesResponse.TimeEntryTemplate.builder()
                                .build())
                        .toList())
                .count(count)
                .build();
    }
}
