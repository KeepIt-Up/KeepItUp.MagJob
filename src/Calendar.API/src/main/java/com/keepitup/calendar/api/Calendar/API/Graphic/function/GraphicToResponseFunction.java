package com.keepitup.calendar.api.Calendar.API.Graphic.function;

import com.keepitup.calendar.api.Calendar.API.Graphic.dto.GetGraphicResponse;
import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class GraphicToResponseFunction implements Function<Graphic, GetGraphicResponse> {

    @Override
    public GetGraphicResponse apply(Graphic graphic) {
        return GetGraphicResponse.builder()
                .id(graphic.getId())
                .name(graphic.getName())
                .managerId(graphic.getManagerId())
                .timeEntryMembers(graphic.getTimeEntryMembers())
                .timeEntries(graphic.getTimeEntries())
                .build();
    }
}
