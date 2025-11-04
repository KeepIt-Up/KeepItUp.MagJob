package com.keepitup.calendar.api.Calendar.API.timeentry.function;

import com.keepitup.calendar.api.Calendar.API.timeentry.dto.PatchTimeEntryRequest;
import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class UpdateTimeEntryWithRequestFunction implements BiFunction<TimeEntry, PatchTimeEntryRequest, TimeEntry> {
    @Override
    public TimeEntry apply(TimeEntry timeEntry, PatchTimeEntryRequest request) {
        return TimeEntry.builder()
                .id(timeEntry.getId())
                .startDateTime(request.getStartDateTime())
                .endDateTime(request.getEndDateTime())
                .build();
    }
}
