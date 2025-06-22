package com.keepitup.calendar.api.Calendar.API.timeentrymember.function;

import com.keepitup.calendar.api.Calendar.API.timeentrymember.dto.PostTimeEntryMemberRequest;
import com.keepitup.calendar.api.Calendar.API.timeentrymember.entity.TimeEntryMember;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class RequestToTimeEntryMemberFunction implements Function<PostTimeEntryMemberRequest, TimeEntryMember> {
    @Override
    public TimeEntryMember apply(PostTimeEntryMemberRequest request) {
        return TimeEntryMember.builder()
                .timeEntry(request.getTimeEntry())
                .member(request.getMember())
                .status(request.getStatus())
                .build();
    }
}
