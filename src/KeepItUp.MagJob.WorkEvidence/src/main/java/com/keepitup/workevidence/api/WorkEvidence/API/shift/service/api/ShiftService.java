package com.keepitup.workevidence.api.WorkEvidence.API.shift.service.api;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;

import java.util.Optional;
import java.util.UUID;


public interface ShiftService {

    Optional<Shift> startShift(Shift shift);

    Optional<Shift> endShift(UUID shiftId);

    void deleteShift(UUID shiftId);

    Optional<Shift> findById(UUID shiftId);

    Optional<Shift> getActiveShifts(UUID userId);

    Optional<java.util.List<Shift>> getAllShifts(UUID memberId);

}
