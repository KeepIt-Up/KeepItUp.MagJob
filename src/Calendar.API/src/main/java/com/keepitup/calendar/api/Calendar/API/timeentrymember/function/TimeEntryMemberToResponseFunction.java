package com.keepitup.calendar.api.Calendar.API.timeentrymember.function;

import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.GetTimeEntryMemberResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class TimeEntryMemberToResponseFunction implements Function<TimeEntryMember, GetTimeEntryMemberResponse> {

    @Override
    public GetTimeEntryMemberResponse apply(TimeEntryMember timeEntryMember) {
        return GetTimeEntryMemberResponse.builder()
                .status(timeEntryMember.getStatus())
                .timeEntry(timeEntryMember.getTimeEntry())
                .member(timeEntryMember.getMember())
                .build();
    }
}
