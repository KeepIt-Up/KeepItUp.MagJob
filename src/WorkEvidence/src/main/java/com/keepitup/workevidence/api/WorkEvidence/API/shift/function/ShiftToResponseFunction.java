package com.keepitup.workevidence.api.WorkEvidence.API.shift.function;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.GetShiftResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestsResponse;
import org.springframework.stereotype.Component;

import java.util.List;
import java.util.function.Function;
import java.util.stream.Collectors;

@Component
public class ShiftToResponseFunction implements Function<Shift, GetShiftResponse> {

    @Override
    public GetShiftResponse apply(Shift shift) {
        List<GetShiftEditRequestResponse> shiftEditRequestResponse = shift.getShiftEditRequests().stream()
                .map(shiftEditRequest -> GetShiftEditRequestResponse.builder()
                        .id(shiftEditRequest.getId())
                        .status(shiftEditRequest.getStatus())
                        .startTime(shiftEditRequest.getStartTime())
                        .endTime(shiftEditRequest.getEndTime())
                        .build())
                .collect(Collectors.toList());

        return GetShiftResponse.builder()
                .id(shift.getId())
                .startTime(shift.getStartTime())
                .endTime(shift.getEndTime())
                .description(shift.getDescription())
                .member(shift.getMember())
                .shiftEditRequests(shiftEditRequestResponse)
                .build();
    }
}