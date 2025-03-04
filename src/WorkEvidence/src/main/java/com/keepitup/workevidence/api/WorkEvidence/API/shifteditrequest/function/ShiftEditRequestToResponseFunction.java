package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function;

import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity.ShiftEditRequest;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class ShiftEditRequestToResponseFunction implements Function<ShiftEditRequest, GetShiftEditRequestResponse> {
    @Override
    public GetShiftEditRequestResponse apply(ShiftEditRequest shiftEditRequest) {
        return GetShiftEditRequestResponse.builder()
                .id(shiftEditRequest.getId())
                .status(shiftEditRequest.getStatus())
                .startTime(shiftEditRequest.getStartTime())
                .endTime(shiftEditRequest.getEndTime())
                .build();
    }
}