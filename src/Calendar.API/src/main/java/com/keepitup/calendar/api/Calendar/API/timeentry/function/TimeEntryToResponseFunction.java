package com.keepitup.calendar.api.Calendar.API.timeentry.function;

import com.keepitup.calendar.api.Calendar.API.timeentry.dto.GetTimeEntryResponse;
import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class TimeEntryToResponseFunction implements Function<TimeEntry, GetTimeEntryResponse> {

    @Override
    public GetTimeEntryResponse apply(TimeEntry timeEntry) {
        return GetTimeEntryResponse.builder()
                .startDateTime(timeEntry.getStartDateTime())
                .endDateTime(timeEntry.getEndDateTime())
                .id(timeEntry.getId())
                .build();
    }
}
