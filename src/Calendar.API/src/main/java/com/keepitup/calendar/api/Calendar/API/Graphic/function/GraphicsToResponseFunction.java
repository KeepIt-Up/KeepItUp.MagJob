package com.keepitup.calendar.api.Calendar.API.Graphic.function;

import com.keepitup.calendar.api.Calendar.API.Graphic.dto.GetGraphicResponse;
import com.keepitup.calendar.api.Calendar.API.Graphic.dto.GetGraphicsResponse;
import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class GraphicsToResponseFunction implements BiFunction<Page<Graphic>, Integer, GetGraphicsResponse> {

    @Override
    public GetGraphicsResponse apply(Page<Graphic> entities, Integer count) {
        return GetGraphicsResponse.builder()
                .graphicsResponse(
                        entities
                                .stream()
                                .map(timeEntry -> GetGraphicResponse.builder().build())
                                .toList()
                )
                .count(count)
                .build();
    }
}
