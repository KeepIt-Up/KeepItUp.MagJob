package com.keepitup.calendar.api.Calendar.API.Graphic.function;

import com.keepitup.calendar.api.Calendar.API.Graphic.dto.PatchGraphicRequest;
import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class UpdateGraphicWithRequestFunction implements BiFunction<Graphic, PatchGraphicRequest, Graphic> {
    @Override
    public Graphic apply(Graphic graphic, PatchGraphicRequest request) {
        return Graphic.builder()
                .id(graphic.getId())
                .name(graphic.getName())
                .managerId(graphic.getManagerId())
                .timeEntryMembers(graphic.getTimeEntryMembers())
                .build();
    }
}
