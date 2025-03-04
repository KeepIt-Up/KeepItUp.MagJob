package com.keepitup.calendar.api.Calendar.API.timeentrymember.function;

import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.PatchTimeEntryMemberRequest;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class UpdateTimeEntryMemberWithRequestFunction implements BiFunction<TimeEntryMember, PatchTimeEntryMemberRequest, TimeEntryMember> {
    @Override
    public TimeEntryMember apply(TimeEntryMember timeEntryMember, PatchTimeEntryMemberRequest request) {
        return TimeEntryMember.builder()
                .id(timeEntryMember.getId())
                .timeEntry(timeEntryMember.getTimeEntry())
                .member(timeEntryMember.getMember())
                .status(timeEntryMember.getStatus())
                .build();
    }
}
