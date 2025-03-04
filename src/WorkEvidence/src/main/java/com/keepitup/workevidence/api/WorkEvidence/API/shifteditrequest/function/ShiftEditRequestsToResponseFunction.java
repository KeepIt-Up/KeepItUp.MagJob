package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function;

import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestsResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity.ShiftEditRequest;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class ShiftEditRequestsToResponseFunction implements BiFunction<Page<ShiftEditRequest>,Integer, GetShiftEditRequestsResponse> {
    @Override
    public GetShiftEditRequestsResponse apply(Page<ShiftEditRequest> entities,Integer count) {
        return GetShiftEditRequestsResponse.builder()
                .shiftEditRequests(entities.stream()
                        .map(shiftEditRequest -> GetShiftEditRequestsResponse.ShiftEditRequest.builder()
                                .id(shiftEditRequest.getId())
                                .status(shiftEditRequest.getStatus())
                                .startTime(shiftEditRequest.getStartTime())
                                .endTime(shiftEditRequest.getEndTime())
                                .build())
                        .toList())
                .count(count)
                .build();


    }
}