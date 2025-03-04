package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function;

import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.GetAvailabilityTemplateResponse;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.GetAvailabilityTemplatesResponse;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;
import java.util.function.BiFunction;

@Component
public class AvailabilityTemplatesToResponseFunction implements BiFunction<Page<AvailabilityTemplate>, Integer, GetAvailabilityTemplatesResponse> {

    @Override
    public GetAvailabilityTemplatesResponse apply(Page<AvailabilityTemplate> entities, Integer count) {
        List<GetAvailabilityTemplateResponse> availabilityTemplateResponseList = new ArrayList<GetAvailabilityTemplateResponse>();
        for (AvailabilityTemplate availabilityTemplate: entities){
            availabilityTemplateResponseList.add(
                    GetAvailabilityTemplateResponse.builder()
                            .id(availabilityTemplate.getId())
                            .name(availabilityTemplate.getName())
                            .numberOfDays(availabilityTemplate.getNumberOfDays())
                            .organizationId(availabilityTemplate.getOrganizationId())
                            .startDayOfWeek(availabilityTemplate.getStartDayOfWeek())
                            .timeEntryTemplates(availabilityTemplate.getTimeEntryTemplates())
                            .build()
            );
        }

        return GetAvailabilityTemplatesResponse.builder()
                .availabilityTemplateResponseList(availabilityTemplateResponseList)
                .build();
    }
}
