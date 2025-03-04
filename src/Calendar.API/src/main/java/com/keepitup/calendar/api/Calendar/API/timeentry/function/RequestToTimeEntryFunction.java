package com.keepitup.calendar.api.Calendar.API.timeentry.function;

import com.keepitup.calendar.api.Calendar.API.timeentry.dto.PostTimeEntryRequest;
import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class RequestToTimeEntryFunction implements Function<PostTimeEntryRequest, TimeEntry> {
    @Override
    public TimeEntry apply(PostTimeEntryRequest request) {
        return TimeEntry.builder()
                .id(request.getId())
                .startDateTime(request.getStartDateTime())
                .endDateTime(request.getEndDateTime())
                .build();
    }
}
