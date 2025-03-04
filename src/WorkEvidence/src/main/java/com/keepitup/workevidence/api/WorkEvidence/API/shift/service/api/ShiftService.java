package com.keepitup.workevidence.api.WorkEvidence.API.shift.service.api;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;

import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

public interface ShiftService {

    Optional<Shift> startShift(Shift shift);

    Optional<Shift> endShift(BigInteger shiftId, Shift shift);

    void deleteShift(BigInteger shiftId);

    Optional<Shift> findById(BigInteger shiftId);

}
