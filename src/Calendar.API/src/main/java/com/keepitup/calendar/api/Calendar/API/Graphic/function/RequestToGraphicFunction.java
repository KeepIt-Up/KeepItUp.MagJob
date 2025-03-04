package com.keepitup.calendar.api.Calendar.API.Graphic.function;

import com.keepitup.calendar.api.Calendar.API.Graphic.dto.PostGraphicRequest;
import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class RequestToGraphicFunction implements Function<PostGraphicRequest, Graphic> {
    @Override
    public Graphic apply(PostGraphicRequest request) {
        return Graphic.builder()
                .name(request.getName())
                .managerId(request.getManagerId())
                .timeEntryMembers(request.getTimeEntryMembers())
                .build();
    }
}
