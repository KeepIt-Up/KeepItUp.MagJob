package com.keepitup.calendar.api.Calendar.API.timeentrymember.function;

import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.GetTimeEntryMembersResponse;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class TimeEntryMembersToResponseFunction implements BiFunction<Page<TimeEntryMember>, Integer, GetTimeEntryMembersResponse> {

    @Override
    public GetTimeEntryMembersResponse apply(Page<TimeEntryMember> entities, Integer count) {
        return GetTimeEntryMembersResponse.builder()
                .timeEntryMemberList(entities.stream()
                        .map(organization -> GetTimeEntryMembersResponse.TimeEntryMember.builder()
                                .build())
                        .toList())
                .count(count)
                .build();
    }
}
