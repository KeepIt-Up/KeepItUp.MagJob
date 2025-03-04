package com.keepitup.workevidence.api.WorkEvidence.API.shift.function;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.PostStartShiftRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class RequestToShiftFunction implements Function<PostStartShiftRequest, Shift> {
    @Override
    public Shift apply(PostStartShiftRequest request) {
        if (request == null || request.getStartTime() == null) {
            throw new IllegalArgumentException("Request and required fields cannot be null");
        }

        return Shift.builder()
                .startTime(request.getStartTime())
                .description(request.getDescription())
                .build();
    }
}