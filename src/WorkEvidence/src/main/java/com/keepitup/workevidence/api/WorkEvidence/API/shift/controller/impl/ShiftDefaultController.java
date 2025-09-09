package com.keepitup.workevidence.api.WorkEvidence.API.shift.controller.impl;

import com.keepitup.workevidence.api.WorkEvidence.API.shift.controller.api.ShiftController;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.GetEndShiftResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.GetShiftResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.GetShiftsResponse;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.PatchEndShiftRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.dto.PostStartShiftRequest;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.entity.Shift;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.function.*;
import com.keepitup.workevidence.api.WorkEvidence.API.shift.service.api.ShiftService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;
import java.util.List;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageImpl;

@RestController
@Log
public class ShiftDefaultController implements ShiftController {
    private final ShiftService service;
    private final ShiftToResponseFunction shiftToResponse;
    private final ShiftsToResponseFunction shiftsToResponse;
    private final RequestToShiftFunction requestToShift;
    private final UpdateShiftWithRequestFunction updateShiftWithRequest;

    @Autowired
    public ShiftDefaultController(
            ShiftService service,
            ShiftToResponseFunction shiftToResponse,
            ShiftsToResponseFunction shiftsToResponse,
            RequestToShiftFunction requestToShift,
            UpdateShiftWithRequestFunction updateShiftWithRequest
    ) {
        this.service = service;
        this.shiftToResponse = shiftToResponse;
        this.shiftsToResponse = shiftsToResponse;
        this.requestToShift = requestToShift;
        this.updateShiftWithRequest = updateShiftWithRequest;
    }

    @Override
    public GetShiftResponse startShift(PostStartShiftRequest postShiftRequest) {
        Shift shift = requestToShift.apply(postShiftRequest);
        service.startShift(shift);
        GetShiftResponse response = new GetShiftResponse();
        response.setId(shift.getId());  // Zwracamy ID zapisanej zmiany
        response.setStartTime(shift.getStartTime());
        response.setEndTime(shift.getEndTime());
        response.setDescription(shift.getDescription());

        return response;
    }

    @Override
    public void deleteShift(UUID id) {
        Optional<Shift> shift = service.findById(id);

        if(shift.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Shift not found");
        }
        service.deleteShift(id);
    }

    @Override
    public GetEndShiftResponse endShift(UUID id) {
        Optional<Shift> shift = service.findById(id);

        if (shift.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Shift not found");
        }
        service.endShift(id);
        return null;
    }

    @Override
    public GetShiftResponse getShift(UUID id) {
        Optional<Shift> shift = service.findById(id);

        if (shift.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Shift not found");
        }
        return shiftToResponse.apply(shift.get());
    }

    @Override
    public GetShiftResponse getActiveShifts(UUID memberId) {
        Optional<Shift> shift = service.getActiveShifts(memberId);
        if (shift.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Shift not found");
        }
        return shiftToResponse.apply(shift.get());
    }

    @Override
    public GetShiftsResponse getAllShifts(UUID memberId) {
        Optional<List<Shift>> shifts = service.getAllShifts(memberId);
        if (shifts.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Shifts not found");
        }
        return shiftsToResponse.apply(shifts.get());
    }
}
