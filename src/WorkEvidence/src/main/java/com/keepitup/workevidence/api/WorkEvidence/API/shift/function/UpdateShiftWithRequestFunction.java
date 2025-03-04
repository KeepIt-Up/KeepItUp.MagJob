package com.keepitup.workevidence.api.WorkEvidence.API.shift.function;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.PatchEndShiftRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class UpdateShiftWithRequestFunction implements BiFunction<Shift, PatchEndShiftRequest, Shift> {
    @Override
    public Shift apply(Shift shift, PatchEndShiftRequest request) {
        return Shift.builder()
                .id(shift.getId())
                .endTime(request.getEndTime())
                .description(request.getNotes())
                .build();
    }
}