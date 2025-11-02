package com.keepitup.workevidence.api.WorkEvidence.API.shift.function;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.GetShiftResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.GetShiftsResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.GetShiftEditRequestResponse;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Component;

import java.util.function.Function;
import java.util.List;
import java.util.stream.Collectors;

@Component
public class ShiftsToResponseFunction implements Function<List<Shift>, GetShiftsResponse> {
    @Override
    public GetShiftsResponse apply(List<Shift> entities) {
        return GetShiftsResponse.builder()
                .shifts(entities.stream()
                        .map(shift -> GetShiftsResponse.Shift.builder()
                                .id(shift.getId())
                                .startTime(shift.getStartTime())
                                .endTime(shift.getEndTime())
                                .description(shift.getDescription())
                                .status(shift.isStatus())
                                .memberId(shift.getMemberId())
                                .shiftEditRequests(shift.getShiftEditRequests().stream()
                                        .map(shiftEditRequest -> GetShiftEditRequestResponse.builder()
                                                .id(shiftEditRequest.getId())
                                                .status(shiftEditRequest.getStatus())
                                                .startTime(shiftEditRequest.getStartTime())
                                                .endTime(shiftEditRequest.getEndTime())
                                                .build())
                                        .collect(Collectors.toList()))
                                .build())
                        .collect(Collectors.toList()))
                .count(entities.size())
                .build();
    }
}