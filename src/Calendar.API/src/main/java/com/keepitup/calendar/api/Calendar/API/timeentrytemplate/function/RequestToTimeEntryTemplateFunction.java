package com.keepitup.calendar.api.Calendar.API.timeentrytemplate.function;

import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.dto.PostTimeEntryTemplateRequest;
import com.keepitup.calendar.api.Calendar.API.timeentrytemplate.entity.TimeEntryTemplate;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class RequestToTimeEntryTemplateFunction implements Function<PostTimeEntryTemplateRequest, TimeEntryTemplate> {
    @Override
    public TimeEntryTemplate apply(PostTimeEntryTemplateRequest request) {
        return TimeEntryTemplate.builder()
                .id(request.getId())
                .startDayOffset(request.getStartDayOffset())
                .endDayOffset(request.getEndDayOffset())
                .startTime(request.getStartTime())
                .endTime(request.getEndTime())
                .build();
    }
}
