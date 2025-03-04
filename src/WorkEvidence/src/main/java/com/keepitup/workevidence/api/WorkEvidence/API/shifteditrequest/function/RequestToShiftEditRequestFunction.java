package com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.function;

import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.dto.PostShiftEditRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shifteditrequest.entity.ShiftEditRequest;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class RequestToShiftEditRequestFunction implements Function<PostShiftEditRequest, ShiftEditRequest> {
    @Override
    public ShiftEditRequest apply(PostShiftEditRequest postShiftEditRequest) {
        return ShiftEditRequest.builder()
                .status(postShiftEditRequest.getStatus())
                .startTime(postShiftEditRequest.getNewStartTime())
                .endTime(postShiftEditRequest.getNewEndTime())
                .build();
    }
}
