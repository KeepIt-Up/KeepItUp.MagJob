package com.keepitup.calendar.api.Calendar.API.timeentry.function;

import com.keepitup.calendar.api.Calendar.API.timeentry.dto.GetTimeEntrysResponse;
import com.keepitup.calendar.api.Calendar.API.timeentry.entity.TimeEntry;
import com.keepitup.calendar.api.Calendar.API.timeentry.dto.GetTimeEntrysResponse;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Component;

import java.util.Collection;
import java.util.function.BiFunction;

@Component
public class TimeEntrysToResponseFunction implements BiFunction<Page<TimeEntry>, Integer, GetTimeEntrysResponse> {

    @Override
    public GetTimeEntrysResponse apply(Page<TimeEntry> entities, Integer count) {
        return GetTimeEntrysResponse.builder()
                .timeEntryList(entities.stream()
                        .map(timeEntry -> GetTimeEntrysResponse.TimeEntry.builder()
                                .build())
                        .toList())
                .count(count)
                .build();
    }
}
