package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function;

import com.keepitup.calendar.api.Calendar.API.Graphic.dto.PostGraphicRequest;
import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component

public class PostCreateAndPopulateGraphicToResponseFunction implements BiFunction<Graphic, PostGraphicRequest, Graphic> {

    @Override
    public Graphic apply(Graphic graphic, PostGraphicRequest postGraphicRequest) {
        return Graphic.builder()
                .name(postGraphicRequest.getName())
                .timeEntries(postGraphicRequest.getTimeEntries())
                .timeEntryMembers(postGraphicRequest.getTimeEntryMembers())
                .managerId(postGraphicRequest.getManagerId())
                .build();
    }
}
