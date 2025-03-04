package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function;

import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.PatchShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity.ShiftEditRequest;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class UpdateShiftEditRequestFunction implements BiFunction<ShiftEditRequest, PatchShiftEditRequest, ShiftEditRequest> {
    @Override
    public ShiftEditRequest apply(ShiftEditRequest shiftEditRequest, PatchShiftEditRequest patchShiftEditRequest) {
        return ShiftEditRequest.builder()
                .id(shiftEditRequest.getId())
                .status(patchShiftEditRequest.getStatus())
                .startTime(patchShiftEditRequest.getStartTime())
                .endTime(patchShiftEditRequest.getEndTime())
                .build();
    }
}
